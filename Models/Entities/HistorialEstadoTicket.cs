namespace MesaDeAyuda.Models.Entities
{
    public class HistorialEstadoTicket
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public int EstadoAnteriorId { get; set; }
        public int EstadoNuevoId { get; set; }
        public int UsuarioId { get; set; }

        public DateTime Fecha { get; set; }

        public Ticket Ticket { get; set; }
        public Estado EstadoAnterior { get; set; }
        public Estado EstadoNuevo { get; set; }
        public Usuario Usuario { get; set; }
    }
}
