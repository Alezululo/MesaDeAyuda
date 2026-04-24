namespace MesaDeAyuda.Models.Entities
{
    public class TicketAdjunto
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }

        public DateTime FechaSubida { get; set; }

        public Ticket Ticket { get; set; }
    }
}