using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinalEntra21.Models
{
	public class TipoEvento
	{
		[Key]
		[DisplayName("Id")]
		public int Id { get; set; }

		[DisplayName("Tipo de evento")]
		public string Tipo { get; set; }
	}
}
