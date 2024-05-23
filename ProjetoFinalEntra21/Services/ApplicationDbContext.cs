using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetoFinalEntra21.Models;

namespace ProjetoFinalEntra21.Services
{
    public class ApplicationDbContext : IdentityDbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Evento>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<Genero>().HasData(
                    new Genero { Id = 1, Nome = "Masculino" },
                    new Genero { Id = 2, Nome = "Feminino" },
                    new Genero { Id = 3, Nome = "Neutro" });
            modelBuilder.Entity<Mensagem>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<StatusEvento>().HasData(
                    new StatusEvento { Id = 1, Nome = "Aberto" },
                    new StatusEvento { Id = 2, Nome = "Fechado" });
            modelBuilder.Entity<StatusMensagem>().HasData(
                    new StatusEvento { Id = 1, Nome = "Aprovada" },
                    new StatusEvento { Id = 2, Nome = "Reprovada" },
                    new StatusEvento { Id = 3, Nome = "Pendente" });
            modelBuilder.Entity<TipoEvento>().HasData(
                    new TipoEvento { Id = 1, Tipo = "Aniversário" },
                    new TipoEvento { Id = 2, Tipo = "Tempo de empresa" },
                    new TipoEvento { Id = 3, Tipo = "Casamento" },
                    new TipoEvento { Id = 4, Tipo = "Natal" });
            modelBuilder.Entity<TipoUsuarios>().HasData(
                    new TipoEvento { Id = 1, Tipo = "Administrador" },
                    new TipoEvento { Id = 2, Tipo = "Convidado" },
                    new TipoEvento { Id = 3, Tipo = "Destinatário" });
            modelBuilder.Entity<Usuario>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<UsuarioEvento>().Property(p => p.Id).UseIdentityAlwaysColumn();

        }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
        public DbSet<StatusEvento> StatusEventos { get; set; }
        public DbSet<StatusMensagem> StatusMensagens { get; set; }
        public DbSet<TipoEvento> TiposEventos { get; set; }
        public DbSet<TipoUsuarios> TipoUsuarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioEvento> UsuarioEvento { get; set; }

    }
}
