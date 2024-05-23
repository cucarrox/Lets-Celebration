using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinalEntra21.Models;
using ProjetoFinalEntra21.Services;
using System.Net;
using System.Net.Mail;

namespace ProjetoFinalEntra21.Controllers
{
    public class MensagensController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MensagensController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensagem = await _context.Mensagens
                .Include(m => m.Autor)
                .Include(m => m.Evento)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mensagem == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.StatusMensagens, "Id", "Nome");
            return View(mensagem);
        }

        public IActionResult Adicionar([FromQuery] int eventoId)
        {
            ViewData["AutorId"] = new SelectList(_context.Usuarios, "Id", "Nome");
            ViewData["StatusId"] = new SelectList(_context.StatusMensagens, "Id", "Nome");
            ViewData["EventoId"] = eventoId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(int eventoId, Mensagem mensagem)
        {
            //Pegar o Id do usuário logado
            var IdUsuarioLogado = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == IdUsuarioLogado.Email.ToUpper());

            var status = await _context.StatusMensagens.FirstOrDefaultAsync(s => s.Nome == "Pendente");
            var evento = await _context.Eventos
                .Include(s => s.Status)
                .Include(a => a.Administrador)
                .FirstOrDefaultAsync(u => u.Id == eventoId);

            if (usuarioLogado != evento.Destinatario && evento.Status.Nome == "Aberto")
            {
                Mensagem novaMensagem = new Mensagem
                {
                    Id = mensagem.Id,
                    Corpo = mensagem.Corpo,
                    Autor = usuarioLogado,
                    Data = DateTime.UtcNow,
                    Status = status,
                    Evento = evento
                };

                _context.Add(novaMensagem);
                await _context.SaveChangesAsync();

                if (novaMensagem.Status.Nome == "Pendente")
                {
                    // Enviar mensagem para aprovação por e-mail
                    var mail = "integrador@fksmart.com.br";
                    var pw = "{~Z!8D(LeXba";

                    var client = new SmtpClient("mail.fksmart.com.br", 587)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(mail, pw)
                    };

                    // Construindo a mensagem de e-mail
                    var message = "Confira a mensagem em: " + "letscelebration.com.br/Eventos/Detalhes/" + evento.Id;

                    await client.SendMailAsync(
                        new MailMessage(from: mail,
                                        to: evento.Administrador.Email,
                                        "Você recebeu uma nova mensagem para aprovação no evento " + evento.Titulo,
                                        message
                                        ));
                }

                return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
            }
            return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensagem = await _context.Mensagens
                .Include(a => a.Autor)
                .Include(s => s.Status)
                .Include(e => e.Evento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mensagem == null)
            {
                return NotFound();
            }
            return View(mensagem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int EventoId, Mensagem mensagem)
        {
            //Pegar o Id do usuário logado
            var IdUsuarioLogado = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == IdUsuarioLogado.Email.ToUpper());

            var mensagemEditada = await _context.Mensagens.FindAsync(mensagem.Id);

            if (usuarioLogado == mensagemEditada.Autor)
            {
                mensagemEditada.Corpo = mensagem.Corpo;
                mensagemEditada.Autor = mensagemEditada.Autor;
                mensagemEditada.Data = mensagemEditada.Data;
                mensagemEditada.Status = mensagemEditada.Status;
                mensagemEditada.Evento = mensagemEditada.Evento;

                await _context.SaveChangesAsync();
                return RedirectToAction("Detalhes", "Eventos", new { id = EventoId });
            }
            return RedirectToAction("Detalhes", "Eventos", new { id = EventoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprovar(Mensagem mensagem)
        {
            //Pegar o Id do usuário logado
            var IdUsuarioLogado = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == IdUsuarioLogado.Email.ToUpper());

            var novoStatus = await _context.StatusMensagens.FirstOrDefaultAsync(s => s.Nome == "Aprovada");

            var mensagemAprovada = await _context.Mensagens
                .Include(e => e.Evento)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(e => e.Id == mensagem.Id);

            mensagemAprovada.Status = novoStatus;
            var eventoId = mensagemAprovada.Evento.Id;

            if (usuarioLogado == mensagemAprovada.Evento.Administrador)
            {
                if (mensagemAprovada != null)
                {
                    _context.Update(mensagemAprovada);
                    await _context.SaveChangesAsync();

                    if (eventoId != null)
                    {
                        return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
                    }
                }
            }
            return RedirectToAction("Index", "Eventos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reprovar(Mensagem mensagem)
        {
            // Pegar o Id do usuário logado
            var IdUsuarioLogado = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == IdUsuarioLogado.Email.ToUpper());

            var novoStatus = await _context.StatusMensagens.FirstOrDefaultAsync(s => s.Nome == "Reprovada");

            var mensagemReprovada = await _context.Mensagens
                .Include(e => e.Evento)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(e => e.Id == mensagem.Id);

            mensagemReprovada.Status = novoStatus;
            var eventoId = mensagemReprovada.Evento.Id;

            if (usuarioLogado == mensagemReprovada.Evento.Administrador)
            {
                if (mensagemReprovada != null)
                {
                    _context.Update(mensagemReprovada);
                    await _context.SaveChangesAsync();

                    if (eventoId != null)
                    {
                        return RedirectToAction("Detalhes", "Eventos", new { id = mensagemReprovada.Evento.Id });
                    }
                }
            }
            return RedirectToAction("Index", "Eventos");
        }

        public async Task<IActionResult> Excluir(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensagem = await _context.Mensagens
                .Include(m => m.Autor)
                .Include(m => m.Evento)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mensagem == null)
            {
                return NotFound();
            }

            return View(mensagem);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            // Pegar o Id do usuário logado
            var IdUsuarioLogado = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == IdUsuarioLogado.Email.ToUpper());

            var mensagem = await _context.Mensagens
                .Include(a => a.Autor)
                .Include(e => e.Evento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioLogado == mensagem.Autor)
            {
                if (mensagem != null)
                {
                    var eventoId = mensagem.Evento.Id;
                    _context.Mensagens.Remove(mensagem);
                    await _context.SaveChangesAsync();

                    if (eventoId != null)
                    {
                        return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
                    }
                }
                return RedirectToAction("Index", "Eventos");
            }
            return RedirectToAction("Index", "Eventos");
        }

        private bool MensagemExists(int id)
        {
            return _context.Mensagens.Any(e => e.Id == id);
        }

        public IActionResult EnviarMensagens()
        {
            var mensagens = _context.Mensagens
                .Include(a => a.Autor)
                .Include(s => s.Status)
                .Include(e => e.Evento)
                    .ThenInclude(a => a.Administrador)
                .ToList();

            foreach (var mensagem in mensagens)
            {

            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
