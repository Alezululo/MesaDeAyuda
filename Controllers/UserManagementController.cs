using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MesaDeAyuda.Data;
using MesaDeAyuda.Models.Entities;
using System.Security.Claims;

namespace MesaDeAyuda.Controllers
{
    [Authorize(Policy = "ModuloTI")]
    public class UserManagementController : Controller
    {
        private readonly MesaDeAyudaContext _context;

        public UserManagementController(MesaDeAyudaContext context)
        {
            _context = context;
        }

        // =========================================
        // LISTADO
        // =========================================
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return View(usuarios);
        }

        // =========================================
        // MODAL CREAR
        // =========================================
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var roles = await _context.Roles
                .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Nombre
                })
                .ToListAsync();

            return PartialView("_CrearUsuario", roles);
        }

        // =========================================
        // GUARDAR NUEVO USUARIO
        // =========================================
        [HttpPost]
        public async Task<IActionResult> CrearUsuario(
            string nombre,
            string username,
            string password,
            int rolId,
            IFormFile foto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            string fotoUrl = "/img/users/default-user.png";

            // Guardar foto
            if (foto != null && foto.Length > 0)
            {
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/users");

                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(foto.FileName);
                var rutaCompleta = Path.Combine(ruta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                fotoUrl = "/img/users/" + nombreArchivo;
            }

            var usuario = new Usuario
            {
                Nombre = nombre,
                Username = username,
                RolId = rolId,
                Activo = true,
                FotoUrl = fotoUrl
            };

            var hasher = new PasswordHasher<Usuario>();
            usuario.PasswordHash = hasher.HashPassword(usuario, password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // AUDITORÍA
            _context.AuditoriaUsuarios.Add(new AuditoriaUsuario
            {
                UsuarioAfectadoId = usuario.Id,
                Accion = "Creación",
                Detalle = $"Usuario creado: {username}",
                UsuarioAccionId = userId,
                Fecha = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // =========================================
        // MODAL EDITAR
        // =========================================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null) return NotFound();

            ViewBag.Roles = await _context.Roles
                .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Nombre
                })
                .ToListAsync();

            return PartialView("_EditarUsuario", usuario);
        }

        // =========================================
        // ACTUALIZAR USUARIO
        // =========================================
        [HttpPost]
        public async Task<IActionResult> EditarUsuario(
            int id,
            string nombre,
            string username,
            int rolId,
            bool activo,
            string password,
            IFormFile foto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cambios = new List<string>();

            // Nombre
            if (usuario.Nombre != nombre)
            {
                cambios.Add($"Nombre: {usuario.Nombre} → {nombre}");
                usuario.Nombre = nombre;
            }

            // Username
            if (usuario.Username != username)
            {
                cambios.Add($"Usuario: {usuario.Username} → {username}");
                usuario.Username = username;
            }

            // Rol
            if (usuario.RolId != rolId)
            {
                cambios.Add($"Rol cambiado");
                usuario.RolId = rolId;
            }

            // Estado
            if (usuario.Activo != activo)
            {
                cambios.Add($"Estado: {(usuario.Activo ? "Activo" : "Inactivo")} → {(activo ? "Activo" : "Inactivo")}");
                usuario.Activo = activo;
            }

            // Password
            if (!string.IsNullOrWhiteSpace(password))
            {
                var hasher = new PasswordHasher<Usuario>();
                usuario.PasswordHash = hasher.HashPassword(usuario, password);
                cambios.Add("Contraseña actualizada");
            }

            // Foto
            if (foto != null && foto.Length > 0)
            {
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/users");

                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(foto.FileName);
                var rutaCompleta = Path.Combine(ruta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                usuario.FotoUrl = "/img/users/" + nombreArchivo;
                cambios.Add("Foto actualizada");
            }

            _context.Update(usuario);

            // AUDITORÍA
            if (cambios.Any())
            {
                _context.AuditoriaUsuarios.Add(new AuditoriaUsuario
                {
                    UsuarioAfectadoId = usuario.Id,
                    Accion = "Actualización",
                    Detalle = string.Join(" | ", cambios),
                    UsuarioAccionId = userId,
                    Fecha = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}