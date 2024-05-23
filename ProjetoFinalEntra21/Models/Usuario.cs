using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjetoFinalEntra21.Models
{
	public class Usuario
    {
		[Key]
		[DisplayName("Id")]
		public int Id { get; set; }


        [DisplayName("Nome completo")]
        [Required(ErrorMessage = "Informe o nome")]
        [StringLength(80, ErrorMessage = "O nome deve ter até 80 caracteres")]
        [MinLength(5, ErrorMessage = "O nome deve ter no mínimo 5 caracteres")]
        public string Nome { get; set; }


        [DisplayName("Data de nascimento")]
        [Required(ErrorMessage = "Informe a data de nascimento")]
        public DateOnly DataNascimento { get; set; }


        [DisplayName("Username")]
        [Required(ErrorMessage = "Informe o username")]
        public string Username { get; set; }


        [DisplayName("Gênero")]
        [Required(ErrorMessage = "Informe o gênero")]
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }


        [EmailAddress(ErrorMessage = "Digite um e-mail válido")]
        [Required(ErrorMessage = "Informe o e-mail")]
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "As senhas estão diferentes")]
        public string PasswordConfirmed { get; set; }

    }
}
