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
    public class ConvidadosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ConvidadosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Convidar([FromQuery] int eventoId)
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Include(g => g.Genero), "Id", "Nome");
            ViewData["EventoId"] = eventoId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Convidar(int eventoId, Usuario convidado)
        {
            var evento = await _context.Eventos
                .Include(s => s.Status)
                .Include(d => d.Destinatario)
                .FirstOrDefaultAsync(u => u.Id == eventoId);

            var convidadoCadastrado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == convidado.Email);

            //Pegar o Id do usuário logado
            var usuarioLogadoId = await _userManager.GetUserAsync(User);
            //Pegar o usuário logado
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToUpper() == usuarioLogadoId.Email.ToUpper());

            if (usuarioLogado != evento.Destinatario && evento.Status.Nome == "Aberto")
            {
                //Verificar se convidado está cadastrado
                if (convidadoCadastrado == null)
                {
                    Usuario novoUsuario = new Usuario
                    {
                        Id = convidado.Id,
                        Nome = convidado.Nome,
                        DataNascimento = DateOnly.FromDateTime(DateTime.Today),
                        Username = "",
                        Genero = _context.Generos.FirstOrDefault(u => u.Nome == "Neutro"),
                        Email = convidado.Email,
                        Password = "",
                        PasswordConfirmed = ""
                    };
                    _context.Usuarios.Add(novoUsuario);
                    await _context.SaveChangesAsync();
                    convidadoCadastrado = novoUsuario;
                }

                //Não permitir que o destinatário seja convidado
                if (convidadoCadastrado.Id == evento.Destinatario.Id)
                {
                    ModelState.AddModelError("Email", "Convidado não pode ser o mesmo que o destinatário.");
                    ViewData["EventoId"] = eventoId;
                    return View();
                }

                var convidadoEventoAtual = await _context.TipoUsuarios.FirstOrDefaultAsync(t => t.Tipo == "Convidado");

                //Verificar se usuário já foi convidado no evento atual
                var usuarioConvidado = await _context.UsuarioEvento
                    .Include(u => u.Usuarios)
                    .Where(u => u.Usuarios == convidadoCadastrado)
                    .Include(t => t.TipoUsuarios)
                    .Where(t => t.TipoUsuarios == convidadoEventoAtual)
                    .Include(e => e.Eventos)
                    .Where(e => e.Eventos == evento)
                    .FirstOrDefaultAsync();

                if (usuarioConvidado == null)
                {
                    UsuarioEvento novoConvidado = new UsuarioEvento();
                    novoConvidado.Id = convidado.Id;
                    novoConvidado.Usuarios = convidadoCadastrado;
                    novoConvidado.TipoUsuarios = convidadoEventoAtual;
                    novoConvidado.Eventos = evento;

                    _context.UsuarioEvento.Add(novoConvidado);
                    await _context.SaveChangesAsync();

                    // Enviar convite por e-mail
                    var mail = "integrador@fksmart.com.br";
                    var pw = "{~Z!8D(LeXba";

                    var client = new SmtpClient("mail.fksmart.com.br", 587)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(mail, pw)
                    };

                    // Construindo a mensagem de e-mail
                    var message = "Participe enviando sua mensagem em: " + "letscelebration.com.br/Eventos/Detalhes/" + evento.Id;

                    await client.SendMailAsync(
                        new MailMessage(from: mail,
                                        to: novoConvidado.Usuarios.Email,
                                        "Você foi convidado para participar do evento " + novoConvidado.Eventos.Titulo,
                                        message
                                        ));
                }
            }

            return RedirectToAction("Detalhes", "Eventos", new { id = eventoId });
        }
    }
}
