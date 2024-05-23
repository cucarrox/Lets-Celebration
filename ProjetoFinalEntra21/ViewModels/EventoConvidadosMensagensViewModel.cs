using ProjetoFinalEntra21.Models;

namespace ProjetoFinalEntra21.ViewModels
{
    public class EventoConvidadosMensagensViewModel
    {
        public Evento Evento { get; set; }
        public UsuarioEvento Convidado { get; set; }
        public List<Usuario> ListaConvidados { get; set; }
        public List<Mensagem> ListaMensagens { get; set; }
    }
}
