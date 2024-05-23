using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinalEntra21.Models;
using ProjetoFinalEntra21.Services;
using ProjetoFinalEntra21.ViewModels;

namespace ProjetoFinalEntra21.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var usuarioLogado = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (usuarioLogado.Succeeded)
            {
                return RedirectToAction("Index", "Eventos");
            }

            ModelState.AddModelError(string.Empty, "Login ou senha inválidos.");
            return View(login);

        }

        public IActionResult Register()
        {
            ViewData["GenerosId"] = new SelectList(_context.Generos, "Id", "Nome");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterViewModel register, Usuario usuario)
        {
            // Verificar se o e-mail já está cadastrado
            var existingUser = await _userManager.FindByEmailAsync(register.Usuario.Email);


            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Você já está registrado no sistema.");
            }

            // Verificar se o e-mail já está cadastrado na tabela Usuario
            var emailCadastrado = await _context.Usuarios
                .Include(g => g.Genero)
                .FirstOrDefaultAsync(u => u.Email == register.Usuario.Email);


            if (emailCadastrado != null)
            {
                var novoUsuario = new Usuario()
                {
                    // Atribuir valores do ViewModel
                    Nome = register.Usuario.Nome,
                    DataNascimento = register.Usuario.DataNascimento,
                    Username = register.Usuario.Username,
                    GeneroId = register.Usuario.GeneroId,
                    Email = register.Usuario.Email,
                    Password = register.Usuario.Password, // Use Password do ViewModel ou seria melhor o Identity?
                    PasswordConfirmed = register.Usuario.PasswordConfirmed
                };
                _context.Add(novoUsuario);
            }
            else
            {
                // Atualizar usuário existente
                emailCadastrado.Nome = register.Usuario.Nome;
                emailCadastrado.DataNascimento = register.Usuario.DataNascimento;
                emailCadastrado.Username = register.Usuario.Username;
                emailCadastrado.GeneroId = register.Usuario.GeneroId;
                // Não atualize Email, Username ou Senha se já existirem
            }

            await _context.SaveChangesAsync();

            // Registrar no sistema Identity
            var user = new IdentityUser()
            {
                UserName = register.Usuario.Username,
                Email = register.Usuario.Email
            };
            await _userManager.CreateAsync(user, register.Usuario.Password);
            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Eventos");

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }


}

