using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MesaDeAyuda.Data;
using MesaDeAyuda.Models.Entities;
using MesaDeAyuda.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MesaDeAyuda.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly MesaDeAyudaContext _context;

        public TicketController(MesaDeAyudaContext context)
        {
            _context = context;
        }

        // =========================================
        // USUARIO - CREAR TICKET
        // =========================================
        #region Usuario - Crear Ticket

        [Authorize(Policy = "ModuloUsuario")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateTicketViewModel
            {
                TiposSolicitud = await _context.TiposSolicitud
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Nombre
                    }).ToListAsync(),

                Prioridades = await _context.Prioridades
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    }).ToListAsync()
            };

            return View("CrearTicket", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ModuloUsuario")]
        public async Task<IActionResult> Create(CreateTicketViewModel model, List<IFormFile> archivos)
        {
            if (!ModelState.IsValid)
            {
                model.TiposSolicitud = await _context.TiposSolicitud
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Nombre
                    }).ToListAsync();

                model.Prioridades = await _context.Prioridades
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    }).ToListAsync();

                return View("CrearTicket", model);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var ultimoTicket = await _context.Tickets
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();

            int numero = (ultimoTicket?.Id ?? 0) + 1;
            string codigo = $"TCK-{numero.ToString("D4")}";

            var estadoAbierto = await _context.Estados
                .FirstOrDefaultAsync(e => e.Nombre == "Abierto");

            var ticket = new Ticket
            {
                Codigo = codigo,
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                TipoSolicitudId = model.TipoSolicitudId.Value,
                PrioridadId = model.PrioridadId.Value,
                EstadoActualId = estadoAbierto.Id,
                UsuarioSolicitanteId = userId,
                FechaCreacion = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            if (archivos != null && archivos.Any())
            {
                var rutaBase = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/tickets");

                if (!Directory.Exists(rutaBase))
                    Directory.CreateDirectory(rutaBase);

                foreach (var archivo in archivos)
                {
                    if (archivo.Length > 0)
                    {
                        var nombreArchivo = Guid.NewGuid() + Path.GetExtension(archivo.FileName);
                        var rutaCompleta = Path.Combine(rutaBase, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        _context.TicketAdjuntos.Add(new TicketAdjunto
                        {
                            TicketId = ticket.Id,
                            NombreArchivo = archivo.FileName,
                            RutaArchivo = "/uploads/tickets/" + nombreArchivo,
                            FechaSubida = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Solicitud creada correctamente ({codigo})";

            return RedirectToAction(nameof(MyTickets));
        }

        #endregion

        // =========================================
        // USUARIO - MIS TICKETS
        // =========================================
        #region Usuario - Mis Tickets

        [Authorize(Policy = "ModuloUsuario")]
        public async Task<IActionResult> MyTickets()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tickets = await _context.Tickets
                .Include(t => t.Prioridad)
                .Include(t => t.Estado)
                .Include(t => t.Adjuntos)
                .Include(t => t.TecnicoAsignado)
                .Where(t => t.UsuarioSolicitanteId == userId)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return View("VerTicketsUsuario", tickets);
        }

        [Authorize(Policy = "ModuloUsuario")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var ticket = await _context.Tickets
                .Include(t => t.Prioridad)
                .Include(t => t.Estado)
                .Include(t => t.Adjuntos)
                .Include(t => t.Comentarios)
                    .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioSolicitanteId == userId);

            if (ticket == null)
                return NotFound();

            return PartialView("_DetallesTicketUsuario", ticket);
        }

        #endregion

        // =========================================
        // SOPORTE TI
        // =========================================
        #region Soporte TI

        [Authorize(Policy = "ModuloTI")]
        public async Task<IActionResult> Manage()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Usuario)
                .Include(t => t.Prioridad)
                .Include(t => t.Estado)
                .Include(t => t.TecnicoAsignado)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return View("GestionTicket", tickets);
        }

        [Authorize(Policy = "ModuloTI")]
        public async Task<IActionResult> DetailsAdmin(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Usuario)
                .Include(t => t.Prioridad)
                .Include(t => t.Estado)
                .Include(t => t.Adjuntos)
                .Include(t => t.TecnicoAsignado)
                .Include(t => t.Comentarios)
                    .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            ViewBag.Tecnicos = await _context.Usuarios
                .Where(u => u.Rol.Nombre == "TI")
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Nombre
                }).ToListAsync();

            ViewBag.Estados = await _context.Estados
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                }).ToListAsync();

            ViewBag.Prioridades = await _context.Prioridades
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToListAsync();

            return PartialView("_DetallesTicketAdmin", ticket);
        }

        // =========================================
        // ACTUALIZAR TICKET
        // =========================================
        [HttpPost]
        [Authorize(Policy = "ModuloTI")]
        public async Task<IActionResult> UpdateTicket([FromBody] UpdateTicketViewModel model)
        {
            var ticket = await _context.Tickets.FindAsync(model.Id);

            if (ticket == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol == "TI")
            {
                if (ticket.TecnicoAsignadoId != null && ticket.TecnicoAsignadoId != userId)
                    return Forbid();
            }

            // Auditoría de estado
            if (ticket.EstadoActualId != model.EstadoId)
            {
                _context.HistorialEstadoTicket.Add(new HistorialEstadoTicket
                {
                    TicketId = ticket.Id,
                    EstadoAnteriorId = ticket.EstadoActualId,
                    EstadoNuevoId = model.EstadoId,
                    UsuarioId = userId,
                    Fecha = DateTime.Now
                });
            }

            // Auditoría de asignación
            if (ticket.TecnicoAsignadoId != model.TecnicoId)
            {
                _context.AsignacionesTicket.Add(new AsignacionTicket
                {
                    TicketId = ticket.Id,
                    TecnicoAnteriorId = ticket.TecnicoAsignadoId,
                    TecnicoNuevoId = model.TecnicoId,
                    UsuarioId = userId,
                    Fecha = DateTime.Now
                });
            }

            ticket.TecnicoAsignadoId = model.TecnicoId;
            ticket.EstadoActualId = model.EstadoId;
            ticket.PrioridadId = model.PrioridadId;

            _context.Update(ticket);

            if (!string.IsNullOrWhiteSpace(model.Comentario))
            {
                _context.TicketComentarios.Add(new TicketComentario
                {
                    TicketId = model.Id,
                    UsuarioId = userId,
                    Comentario = model.Comentario,
                    Fecha = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}