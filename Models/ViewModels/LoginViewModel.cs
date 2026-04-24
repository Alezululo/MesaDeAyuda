using System.ComponentModel.DataAnnotations;

namespace MesaDeAyuda.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debe ingresar el usuario.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar la contraseña.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;
    }
}