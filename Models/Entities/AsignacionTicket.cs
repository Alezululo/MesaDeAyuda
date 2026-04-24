namespace MesaDeAyuda.Models.Entities
{
    public class AsignacionTicket
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public int? TecnicoAnteriorId { get; set; }
        public int? TecnicoNuevoId { get; set; }
        public int UsuarioId { get; set; }

        public DateTime Fecha { get; set; }

        public Ticket Ticket { get; set; }
        public Usuario TecnicoAnterior { get; set; }
        public Usuario TecnicoNuevo { get; set; }
        public Usuario Usuario { get; set; }
    }
}
