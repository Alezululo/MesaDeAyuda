using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MesaDeAyuda.Data;

namespace MesaDeAyuda.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly MesaDeAyudaContext _context;

        public AdminController(MesaDeAyudaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AuditoriaTickets()
        {
            ViewBag.UsuariosTickets = await _context.HistorialEstadoTicket
                .Include(h => h.Usuario)
                .Select(h => h.Usuario.Nombre)
                .Union(
                    _context.AsignacionesTicket
                        .Include(a => a.Usuario)
                        .Select(a => a.Usuario.Nombre)
                )
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return View("~/Views/Admin/AuditoriaTickets.cshtml");
        }

        public async Task<IActionResult> AuditoriaUsuarios()
        {
            ViewBag.UsuariosAfectados = await _context.AuditoriaUsuarios
                .Include(a => a.UsuarioAfectado)
                .Select(a => a.UsuarioAfectado.Username)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            ViewBag.AccionesUsuarios = await _context.AuditoriaUsuarios
                .Select(a => a.Accion)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            ViewBag.UsuariosEjecutores = await _context.AuditoriaUsuarios
                .Include(a => a.UsuarioAccion)
                .Select(a => a.UsuarioAccion.Nombre)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return View("~/Views/Admin/AuditoriaUsuarios.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditTicketsData(string? ticket, string? tipo, string? usuario)
        {
            var estados = await _context.HistorialEstadoTicket
                .Include(h => h.Ticket)
                .Include(h => h.Usuario)
                .Select(h => new
                {
                    id = h.Id,
                    ticket = h.Ticket.Codigo,
                    cambioDe = "Estado",
                    detalle = "Cambio de estado",
                    usuario = h.Usuario.Nombre,
                    fecha = h.Fecha
                })
                .ToListAsync();

            var asignaciones = await _context.AsignacionesTicket
                .Include(a => a.Ticket)
                .Include(a => a.Usuario)
                .Select(a => new
                {
                    id = a.Id,
                    ticket = a.Ticket.Codigo,
                    cambioDe = "Asignación",
                    detalle = "Cambio de técnico",
                    usuario = a.Usuario.Nombre,
                    fecha = a.Fecha
                })
                .ToListAsync();

            var result = estados.Concat(asignaciones)
                .OrderByDescending(x => x.fecha)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(ticket))
                result = result.Where(x => x.ticket.Contains(ticket, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(x => x.cambioDe.Equals(tipo, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(usuario))
                result = result.Where(x => x.usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase));

            return Json(result.Select(x => new
            {
                x.id,
                x.ticket,
                cambioDe = x.cambioDe,
                x.detalle,
                x.usuario,
                fecha = x.fecha.ToString("yyyy-MM-dd HH:mm")
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditUsersData(string? usuarioAfectado, string? accion, string? usuarioAccion)
        {
            var data = await _context.AuditoriaUsuarios
                .Include(a => a.UsuarioAfectado)
                .Include(a => a.UsuarioAccion)
                .OrderByDescending(a => a.Fecha)
                .Select(a => new
                {
                    id = a.Id,
                    usuarioAfectado = a.UsuarioAfectado.Username,
                    accion = a.Accion,
                    detalle = a.Detalle,
                    usuarioAccion = a.UsuarioAccion.Nombre,
                    fecha = a.Fecha
                })
                .ToListAsync();

            var result = data.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(usuarioAfectado))
                result = result.Where(x => x.usuarioAfectado.Equals(usuarioAfectado, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(accion))
                result = result.Where(x => x.accion.Equals(accion, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(usuarioAccion))
                result = result.Where(x => x.usuarioAccion.Equals(usuarioAccion, StringComparison.OrdinalIgnoreCase));

            return Json(result.Select(x => new
            {
                x.id,
                x.usuarioAfectado,
                x.accion,
                x.detalle,
                x.usuarioAccion,
                fecha = x.fecha.ToString("yyyy-MM-dd HH:mm")
            }));
        }

        [HttpGet]
        public async Task<IActionResult> AuditTicketDetail(int id, string tipo)
        {
            if (tipo == "Estado")
            {
                var h = await _context.HistorialEstadoTicket
                    .Include(x => x.EstadoAnterior)
                    .Include(x => x.EstadoNuevo)
                    .Include(x => x.Usuario)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (h == null) return NotFound();

                return PartialView("~/Views/Ticket/_DetalleAuditoriaEstado.cshtml", h);
            }
            else
            {
                var a = await _context.AsignacionesTicket
                    .Include(x => x.TecnicoAnterior)
                    .Include(x => x.TecnicoNuevo)
                    .Include(x => x.Usuario)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (a == null) return NotFound();

                return PartialView("~/Views/Ticket/_DetalleAuditoriaAsignacion.cshtml", a);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AuditUserDetail(int id)
        {
            var auditoria = await _context.AuditoriaUsuarios
                .Include(a => a.UsuarioAfectado)
                .Include(a => a.UsuarioAccion)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auditoria == null) return NotFound();

            return PartialView("~/Views/Admin/_DetalleAuditoriaUsuario.cshtml", auditoria);
        }
    }
}