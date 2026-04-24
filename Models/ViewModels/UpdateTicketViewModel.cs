namespace MesaDeAyuda.Models.ViewModels
{
    public class UpdateTicketViewModel
    {
        public int Id { get; set; }
        public int? TecnicoId { get; set; }
        public int EstadoId { get; set; }
        public int PrioridadId { get; set; }
        public string? Comentario { get; set; }
    }
}
