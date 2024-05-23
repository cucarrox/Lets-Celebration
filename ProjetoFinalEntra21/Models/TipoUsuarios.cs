using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoFinalEntra21.Models
{
    [Table("Roles")]
    public class TipoUsuarios
    {
        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Role")]
        public string Tipo { get; set; }
    }
}
