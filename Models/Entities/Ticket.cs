using System.ComponentModel.DataAnnotations.Schema;

namespace MesaDeAyuda.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public int TipoSolicitudId { get; set; }
        public int PrioridadId { get; set; }

        public int EstadoActualId { get; set; }
        public int UsuarioSolicitanteId { get; set; }

        public int? TecnicoAsignadoId { get; set; }
        public Usuario TecnicoAsignado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAsignacion { get; set; }   // cuando TI lo toma
        public DateTime? FechaCierre { get; set; }       // cuando se cierra

        [ForeignKey("TipoSolicitudId")]
        public TipoSolicitud TipoSolicitud { get; set; }

        [ForeignKey("PrioridadId")]
        public Prioridad Prioridad { get; set; }

        [ForeignKey("EstadoActualId")]
        public Estado Estado { get; set; }

        [ForeignKey("UsuarioSolicitanteId")]
        public Usuario Usuario { get; set; }

        public ICollection<TicketAdjunto> Adjuntos { get; set; } = new List<TicketAdjunto>();
        public ICollection<TicketComentario> Comentarios { get; set; } = new List<TicketComentario>();
    }
}