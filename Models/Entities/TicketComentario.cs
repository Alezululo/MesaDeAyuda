namespace MesaDeAyuda.Models.Entities
{
    public class TicketComentario
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string Comentario { get; set; }

        public DateTime Fecha { get; set; }
    }
}