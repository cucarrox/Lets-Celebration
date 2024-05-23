using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinalEntra21.Models
{
	public class Mensagem
	{
		[Key]
		[DisplayName("Id")]
		public int Id { get; set; }


		[DisplayName("Corpo da mensagem")]
		[Required(ErrorMessage = "Escreva uma mensagem")]
		public string Corpo { get; set; }


		[DisplayName("Autor")]
		public Usuario Autor { get; set; }


		[DisplayName("Data da mensagem")]
		[Required(ErrorMessage = "Digite a data da mensagem")]
		[DataType(DataType.DateTime)]
		public DateTime Data { get; set; }


		[DisplayName("Status")]
		public StatusMensagem Status { get; set; }


		[DisplayName("Evento")]
		public Evento Evento { get; set; }
	}
}
