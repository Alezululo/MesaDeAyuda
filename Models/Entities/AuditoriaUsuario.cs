namespace MesaDeAyuda.Models.Entities
{
    public class AuditoriaUsuario
    {
        public int Id { get; set; }

        public int UsuarioAfectadoId { get; set; }
        public int UsuarioAccionId { get; set; }

        public string Accion { get; set; }

        // ESTA ES LA QUE FALTA
        public string Detalle { get; set; }

        public DateTime Fecha { get; set; }

        // Relaciones
        public Usuario UsuarioAfectado { get; set; }
        public Usuario UsuarioAccion { get; set; }
    }
}
