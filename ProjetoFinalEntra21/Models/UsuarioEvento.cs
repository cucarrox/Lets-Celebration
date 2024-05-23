namespace ProjetoFinalEntra21.Models
{
    public class UsuarioEvento
    {
        public int Id { get; set; }
        public Usuario Usuarios { get; set; }
        public TipoUsuarios TipoUsuarios { get; set; }
        public Evento Eventos { get; set; }
    }
}
