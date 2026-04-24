using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MesaDeAyuda.Models.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? FotoUrl { get; set; }

        public bool Activo { get; set; } = true;

        [Required]
        public int RolId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("RolId")]
        public Rol Rol { get; set; } = null!;
    }
}