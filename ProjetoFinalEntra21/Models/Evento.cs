using ProjetoFinalEntra21.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoFinalEntra21.Models
{
	public class Evento
	{
		[Key]
		[DisplayName("Id")]
		public int Id { get; set; }


		[DisplayName("Título")]
		[Required(ErrorMessage = "Digite o título do evento")]
		[StringLength(30, ErrorMessage = "O título deve ter até 30 caracteres")]
		[MinLength(5, ErrorMessage = "O título deve ter no mínimo 5 caracteres")]
		public string Titulo { get; set; }

				
		[DisplayName("Data do evento")]		
		[Required(ErrorMessage = "Digite a data do evento")]
        public DateOnly DataEvento { get; set; }


		[DisplayName("Tipo de evento")]
		[Required(ErrorMessage = "Informe o tipo")]
		public TipoEvento TipoEvento { get; set; }


        [DisplayName("Status do evento")]
		[Required]
        public StatusEvento Status { get; set; }


        [DisplayName("Administrador")]
        [Required(ErrorMessage = "Informe o Administrador")]
        public Usuario Administrador { get; set; }


        [DisplayName("Destinatario")]
        [Required(ErrorMessage = "Informe o Destinatario")]
        public Usuario Destinatario { get; set; }


        [DisplayName("Mensagens")]
        public List<Mensagem> Mensagem { get; set; }

    }
}
