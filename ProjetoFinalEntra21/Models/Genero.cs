using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinalEntra21.Models
{
	public class Genero
	{
		[Key]
		[DisplayName("Id")]
		public int Id { get; set; }


		[DisplayName("Gênero")]
		public string Nome { get; set; }
	}
}
