using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MesaDeAyuda.Models.ViewModels
{
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione un tipo de solicitud")]
        public int? TipoSolicitudId { get; set; }

        [Required(ErrorMessage = "Seleccione una prioridad")]
        public int? PrioridadId { get; set; }

        public List<SelectListItem> TiposSolicitud { get; set; } = new();
        public List<SelectListItem> Prioridades { get; set; } = new();
    }
}