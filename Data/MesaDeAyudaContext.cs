using MesaDeAyuda.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MesaDeAyuda.Data
{
    public class MesaDeAyudaContext : DbContext
    {
        public MesaDeAyudaContext(DbContextOptions<MesaDeAyudaContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAdjunto> TicketAdjuntos { get; set; }
        public DbSet<TicketComentario> TicketComentarios { get; set; }
        public DbSet<TipoSolicitud> TiposSolicitud { get; set; }
        public DbSet<Prioridad> Prioridades { get; set; }
        public DbSet<Estado> Estados { get; set; }

        public DbSet<HistorialEstadoTicket> HistorialEstadoTicket { get; set; }
        public DbSet<AsignacionTicket> AsignacionesTicket { get; set; }

        public DbSet<AuditoriaUsuario> AuditoriaUsuarios { get; set; }
    }
}