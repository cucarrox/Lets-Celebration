using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinalEntra21.Models
{
	public class StatusMensagem
	{
		[Key]
		[DisplayName("Id")]
		public int Id { get; set; }


		[DisplayName("Status da mensagem")]
		public string Nome { get; set; }
	}
}
