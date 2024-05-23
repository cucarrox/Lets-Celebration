using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinalEntra21.Models
{
	public class StatusEvento
	{
		[Key]
		[DisplayName("Id")]
		public int Id { get; set; }

		[DisplayName("Status do evento")]
		public string Nome { get; set; }
	}
}
