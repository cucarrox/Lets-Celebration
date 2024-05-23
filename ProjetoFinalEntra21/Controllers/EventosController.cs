using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinalEntra21.Models;
using ProjetoFinalEntra21.Services;
using ProjetoFinalEntra21.ViewModels;
using System.Net;
using System.Net.Mail;

namespace ProjetoFinalEntra21.Controllers
{
    [Authorize]
    public class EventosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //Pegar o Id do usuário logado
            var administradorId = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var administrador = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == administradorId.Email.ToUpper());
            ViewData["UsuarioLogado"] = administrador;

            var eventosUsuarioLogado = await _context.UsuarioEvento
                .Include(ue => ue.Eventos)
                .Include(t => t.Eventos.TipoEvento)
                .Include(d => d.Eventos.Administrador)
                .Include(d => d.Eventos.Destinatario)
                .Include(s => s.Eventos.Status)
                .Include(u => u.Usuarios)
                .Where(ue => ue.Usuarios == administrador)
                .ToListAsync();

            if (eventosUsuarioLogado == null)
            {
                return View("Index");
            }

            return View("Index", eventosUsuarioLogado);
        }

        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Pegar o usuário logado
            var usuarioLogadoIdentity = await _userManager.GetUserAsync(User);

            //Pegar o usuário que corresponde ao usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == usuarioLogadoIdentity.Email.ToUpper());
            ViewData["UsuarioLogado"] = usuarioLogado;

            //Pegar o evento a partir do id
            var evento = await _context.Eventos
                       .Include(e => e.TipoEvento)
                       .Include(a => a.Administrador)
                       .Include(d => d.Destinatario)
                       .Include(s => s.Status)
                       .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            //Pegar a lista de convidados do evento atual
            var convidados = await _context.UsuarioEvento
                        .Include(t => t.TipoUsuarios)
                        .Where(t => t.TipoUsuarios.Tipo == "Convidado")
                        .Where(c => c.Eventos.Id == id)
                        .Select(c => c.Usuarios)
                        .ToListAsync();

            //Pegar a role do usuario logado no evento atual
            var roleUsuarioAtual = await _context.UsuarioEvento
                .Include(ue => ue.Eventos)
                .Include(u => u.Usuarios)
                .Where(ue => ue.Eventos.Id == id && ue.Usuarios.Id == usuarioLogado.Id)
                .Select(ue => ue.TipoUsuarios.Tipo) // Selecionar o tipo de usuário (role) na tabela UsuarioEvento
                .FirstOrDefaultAsync();

            //Verificar se a role do usuário não é nula
            if (roleUsuarioAtual != null)
            {
                //Verificar se o usuário tem permissão de administrador
                if (roleUsuarioAtual == "Administrador")
                {

                    ViewData["Administrador"] = usuarioLogado;
                    //Usuário tem acesso à todas as mensagens
                    var mensagens = await _context.Mensagens
                        .Include(a => a.Autor)
                        .Include(s => s.Status)
                        .Include(e => e.Evento)
                        .Where(c => c.Evento.Id == id)
                        .OrderByDescending(d => d.Data)
                        .ToListAsync();

                    var viewModel = new EventoConvidadosMensagensViewModel
                    {
                        Evento = evento,
                        ListaConvidados = convidados,
                        ListaMensagens = mensagens
                    };

                    return View(viewModel);

                }
                else if (roleUsuarioAtual == "Convidado")
                {
                    ViewData["Convidado"] = usuarioLogado;
                    //Usuário tem acesso apenas as mensagens enviadas por ele
                    var mensagens = await _context.Mensagens
                        .Include(a => a.Autor)
                        .Where(a => a.Autor == usuarioLogado)
                        .Include(s => s.Status)
                        .Include(e => e.Evento)
                        .Where(c => c.Evento.Id == id)
                        .OrderByDescending(d => d.Data)
                        .ToListAsync();

                    var viewModel = new EventoConvidadosMensagensViewModel
                    {
                        Evento = evento,
                        ListaConvidados = convidados,
                        ListaMensagens = mensagens
                    };

                    return View(viewModel);

                }
                else if (roleUsuarioAtual == "Destinatário")
                {
                    ViewData["Destinatário"] = usuarioLogado;
                    //Usuário tem acesso apenas as mensagens aprovadas
                    var mensagens = await _context.Mensagens
                        .Include(a => a.Autor)
                        .Include(s => s.Status)
                        .Where(s => s.Status.Nome == "Aprovada")
                        .Include(e => e.Evento)
                        .Where(c => c.Evento.Id == id)
                        .OrderByDescending(d => d.Data)
                        .ToListAsync();

                    var viewModel = new EventoConvidadosMensagensViewModel
                    {
                        Evento = evento,
                        ListaConvidados = convidados,
                        ListaMensagens = mensagens
                    };

                    return View(viewModel);
                }
            }
            return RedirectToAction(nameof(Index));
        }


        // ================================================================================================================================================
        public IActionResult Adicionar()
        {
            ViewData["TipoEventoId"] = new SelectList(_context.TiposEventos, "Id", "Tipo");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Include(g => g.Genero), "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(Evento evento)
        {
            //Não permitir a criação de eventos com datas menores ou iguais a hoje
            if (evento.DataEvento <= DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("DataEvento", "A data do evento deve ser no futuro.");
                ViewData["TipoEventoId"] = new SelectList(_context.TiposEventos, "Id", "Tipo");
                return View();
            }

            //Pegar o Id do usuário logado
            var administradorId = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var administrador = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == administradorId.Email.ToUpper());

            //Verificar se destinatário está cadastrado
            var destinatario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == evento.Destinatario.Email.ToUpper());

            if (destinatario == null)
            {
                Usuario novoDestinatario = new Usuario
                {
                    Id = evento.Destinatario.Id,
                    Nome = evento.Destinatario.Nome,
                    DataNascimento = DateOnly.FromDateTime(DateTime.Today),
                    Username = "",
                    Genero = _context.Generos.FirstOrDefault(u => u.Nome == "Neutro"),
                    Email = evento.Destinatario.Email,
                    Password = "",
                    PasswordConfirmed = ""
                };
                _context.Usuarios.Add(novoDestinatario);
                await _context.SaveChangesAsync();
                destinatario = novoDestinatario;
            }

            //Não permitir que o destinatário seja o mesmo que o administrador
            if (administrador.Id == destinatario.Id)
            {
                ModelState.AddModelError("Destinatario.Email", "O destinatário não pode ser o mesmo que o administrador.");
                ViewData["TipoEventoId"] = new SelectList(_context.TiposEventos, "Id", "Tipo");
                return View();
            }

            var tipoEvento = await _context.TiposEventos.FirstOrDefaultAsync(t => t.Id == evento.TipoEvento.Id);

            Evento novoEvento = new Evento
            {
                Id = evento.Id,
                Titulo = evento.Titulo,
                DataEvento = evento.DataEvento,
                TipoEvento = tipoEvento,
                Administrador = administrador,
                Destinatario = destinatario,
                Status = _context.StatusEventos.FirstOrDefault(s => s.Nome == "Aberto")
            };
            _context.Eventos.Add(novoEvento);
            await _context.SaveChangesAsync();

            UsuarioEvento administradorEvento = new UsuarioEvento
            {
                Usuarios = administrador,
                TipoUsuarios = await _context.TipoUsuarios.FirstOrDefaultAsync(t => t.Tipo == "Administrador"),
                Eventos = novoEvento
            };
            _context.UsuarioEvento.Add(administradorEvento);
            await _context.SaveChangesAsync();

            var tipoDestinatario = await _context.TipoUsuarios.FirstOrDefaultAsync(t => t.Tipo == "Destinatário");

            UsuarioEvento destinatarioEvento = new UsuarioEvento
            {
                Usuarios = destinatario,
                TipoUsuarios = tipoDestinatario,
                Eventos = novoEvento
            };
            _context.UsuarioEvento.Add(destinatarioEvento);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }
        // ================================================================================================================================================
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(t => t.TipoEvento)
                .Include(s => s.Status)
                .Include(a => a.Administrador)
                .Include(d => d.Destinatario)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (evento == null)
            {
                return NotFound();
            }
            ViewData["TipoEventoId"] = new SelectList(_context.TiposEventos, "Id", "Tipo", evento.Id);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Include(g => g.Genero), "Id", "Nome", evento.Id);

            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Evento evento)
        {
            // Pegar o Id do usuário logado
            var userLogado = await _userManager.GetUserAsync(User);
            // Pegar o usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == userLogado.Email.ToUpper());

            var eventoEditado = await _context.Eventos.FindAsync(evento.Id);

            if (usuarioLogado.Id == eventoEditado.Administrador.Id)
            {
                eventoEditado.Titulo = evento.Titulo;
                eventoEditado.DataEvento = evento.DataEvento;
                eventoEditado.TipoEvento = await _context.TiposEventos.FindAsync(evento.TipoEvento.Id);

                eventoEditado.Status = eventoEditado.Status;
                eventoEditado.Administrador = eventoEditado.Administrador;
                eventoEditado.Destinatario = eventoEditado.Destinatario;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Chamado editado com sucesso.";

                return RedirectToAction("Detalhes", new { id = evento.Id });
            }
            return RedirectToAction("Editar", new { id = evento.Id });
        }

        public async Task<IActionResult> Excluir(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(t => t.TipoEvento)
                .Include(a => a.Administrador)
                .Include(d => d.Destinatario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            //Pegar o Id do usuário logado
            var administradorId = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var administrador = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == administradorId.Email.ToUpper());

            var evento = await _context.Eventos.FindAsync(id);

            if (administrador == evento.Administrador)
            {
                if (evento != null)
                {
                    _context.Eventos.Remove(evento);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }

        public IActionResult AtualizarStatusEventos()
        {
            var eventos = _context.Eventos
                .Include(e => e.Status)
                .Include(a => a.Administrador)
                .Include(d => d.Destinatario)
                .ToList();

            foreach (var evento in eventos)
            {
                if (evento.DataEvento <= DateOnly.FromDateTime(DateTime.Today) && evento.Status.Nome == "Aberto")
                {
                    evento.Status = _context.StatusEventos.FirstOrDefault(s => s.Nome == "Fechado");

                    // Envia um e-mail
                    EnviarEvento(evento);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private void EnviarEvento(Evento evento)
        {
            var mail = "integrador@fksmart.com.br";
            var pw = "{~Z!8D(LeXba";

            var client = new SmtpClient("mail.fksmart.com.br", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, pw)
            };

            // Construindo a mensagem de e-mail
            var message = "Confira sua homenagem em: " + "letscelebration.com.br/Eventos/Detalhes/" + evento.Id;

            client.SendMailAsync(
                new MailMessage(from: mail,
                                to: evento.Destinatario.Email,
                                "Parabéns! " + evento.Destinatario.Nome + "Você recebeu uma homenagem de " + evento.Administrador.Nome + ".",
                                message
                                ));
        }


    }
}
