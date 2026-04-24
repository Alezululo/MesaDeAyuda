using MesaDeAyuda.Data;
using MesaDeAyuda.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MesaDeAyuda.Controllers
{
    [Authorize(Policy = "ModuloGerencia")]
    public class ReportsController : Controller
    {
        private readonly MesaDeAyudaContext _context;

        public ReportsController(MesaDeAyudaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Tecnicos = await _context.Usuarios
                .Where(u => u.Rol.Nombre == "TI")
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Nombre
                })
                .ToListAsync();

            ViewBag.Estados = await _context.Estados
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                })
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetReportData(DateTime? fechaInicio, DateTime? fechaFin, int? tecnicoId, int? estadoId)
        {
            var query = _context.Tickets
                .Include(t => t.Estado)
                .Include(t => t.TecnicoAsignado)
                .Include(t => t.Comentarios)
                .AsQueryable();

            // FILTROS
            if (fechaInicio.HasValue)
                query = query.Where(t => t.FechaCreacion >= fechaInicio);

            if (fechaFin.HasValue)
                query = query.Where(t => t.FechaCreacion <= fechaFin);

            if (tecnicoId.HasValue)
                query = query.Where(t => t.TecnicoAsignadoId == tecnicoId);

            if (estadoId.HasValue)
                query = query.Where(t => t.EstadoActualId == estadoId);

            var tickets = await query.ToListAsync();

            // KPIs
            var total = tickets.Count;
            var abiertos = tickets.Count(t => t.Estado.Nombre == "Abierto");
            var enProceso = tickets.Count(t => t.Estado.Nombre == "En Proceso");
            var cerrados = tickets.Count(t => t.Estado.Nombre == "Cerrado");

            // Tiempo respuesta
            var tiemposRespuesta = tickets.Select(t =>
            {
                var primerComentario = t.Comentarios
                    .OrderBy(c => c.Fecha)
                    .FirstOrDefault();

                if (primerComentario == null)
                    return (double?)null;

                return (primerComentario.Fecha - t.FechaCreacion).TotalHours;
            })
            .Where(x => x.HasValue)
            .Select(x => x.Value)
            .ToList();

            var promedioRespuesta = tiemposRespuesta.Any() ? tiemposRespuesta.Average() : 0;

            // Tiempo resolución SOLO cerrados
            var tiemposResolucion = tickets.Where(t => t.Estado.Nombre == "Cerrado").Select(t =>
                {
                    var ultimoComentario = t.Comentarios.OrderByDescending(c => c.Fecha).FirstOrDefault();

                    if (ultimoComentario == null)
                        return (double?)null;

                    return (ultimoComentario.Fecha - t.FechaCreacion).TotalHours;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();

            var promedioResolucion = tiemposResolucion.Any()
                ? tiemposResolucion.Average()
                : 0;

            // TABLA
            var tabla = tickets.Select(t => new
            {
                codigo = t.Codigo,
                tecnico = t.TecnicoAsignado != null ? t.TecnicoAsignado.Nombre : "Sin asignar",
                estado = t.Estado.Nombre,
                fecha = t.FechaCreacion.ToString("yyyy-MM-dd"),
                tiempoRespuesta = CalcularHoras(t, true),
                tiempoResolucion = t.Estado.Nombre == "Cerrado"
                    ? CalcularHoras(t, false)
                    : null
            });

            return Json(new
            {
                total,
                abiertos,
                enProceso,
                cerrados,
                promedioRespuesta = Math.Round(promedioRespuesta, 2),
                promedioResolucion = Math.Round(promedioResolucion, 2),
                tabla
            });
        }

        private double? CalcularHoras(Ticket t, bool respuesta)
        {
            if (t.Comentarios == null || !t.Comentarios.Any())
                return null;

            if (respuesta)
            {
                var primero = t.Comentarios
                    .OrderBy(c => c.Fecha)
                    .First();

                return (primero.Fecha - t.FechaCreacion).TotalHours;
            }
            else
            {
                var ultimo = t.Comentarios
                    .OrderByDescending(c => c.Fecha)
                    .First();

                return (ultimo.Fecha - t.FechaCreacion).TotalHours;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(DateTime? fechaInicio, DateTime? fechaFin, int? tecnicoId, int? estadoId)
        {
            var query = _context.Tickets
                .Include(t => t.TecnicoAsignado)
                .Include(t => t.Estado)
                .AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(t => t.FechaCreacion >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(t => t.FechaCreacion <= fechaFin.Value);

            if (tecnicoId.HasValue)
                query = query.Where(t => t.TecnicoAsignadoId == tecnicoId);

            if (estadoId.HasValue)
                query = query.Where(t => t.EstadoActualId == estadoId);

            var data = await query
                .Select(t => new
                {
                    t.Codigo,
                    Tecnico = t.TecnicoAsignado != null ? t.TecnicoAsignado.Nombre : "Sin asignar",
                    Estado = t.Estado.Nombre,
                    Fecha = t.FechaCreacion,
                    TiempoRespuesta = _context.TicketComentarios.Where(c => c.TicketId == t.Id).Select(c => (DateTime?)c.Fecha).Min(),
                    TiempoCierre = t.Estado.Nombre == "Cerrado"
                        ? _context.TicketComentarios.Where(c => c.TicketId == t.Id).Select(c => (DateTime?)c.Fecha).Max()
                        : null
                })
                .ToListAsync();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Reporte");

                ws.Cell(1, 1).Value = "Código";
                ws.Cell(1, 2).Value = "Técnico";
                ws.Cell(1, 3).Value = "Estado";
                ws.Cell(1, 4).Value = "Fecha";
                ws.Cell(1, 5).Value = "Tiempo Respuesta (h)";
                ws.Cell(1, 6).Value = "Tiempo Resolución (h)";

                int fila = 2;

                foreach (var t in data)
                {
                    double? tiempoRespuesta = t.TiempoRespuesta.HasValue
                        ? (t.TiempoRespuesta.Value - t.Fecha).TotalHours
                        : null;

                    double? tiempoResolucion = (t.TiempoCierre.HasValue)
                        ? (t.TiempoCierre.Value - t.Fecha).TotalHours
                        : null;

                    ws.Cell(fila, 1).Value = t.Codigo;
                    ws.Cell(fila, 2).Value = t.Tecnico;
                    ws.Cell(fila, 3).Value = t.Estado;
                    ws.Cell(fila, 4).Value = t.Fecha.ToString("yyyy-MM-dd");
                    ws.Cell(fila, 5).Value = tiempoRespuesta?.ToString("0.00") ?? "-";
                    ws.Cell(fila, 6).Value = tiempoResolucion?.ToString("0.00") ?? "-";

                    fila++;
                }

                int totalFilas = data.Count + 1;

                // =========================
                // 🔥 ESTILOS
                // =========================

                // Ajustar ancho automático
                ws.Columns().AdjustToContents();

                // Centrar contenido
                ws.RangeUsed().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                ws.RangeUsed().Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;

                // Header (negrilla + verde)
                var header = ws.Range(1, 1, 1, 6);
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.SeaGreen;
                header.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

                // Filas alternadas
                for (int i = 2; i <= totalFilas; i++)
                {
                    var filaRango = ws.Range(i, 1, i, 6);

                    if (i % 2 == 0)
                        filaRango.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGreen;
                    else
                        filaRango.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.White;
                }

                // Bordes
                var rango = ws.Range(1, 1, totalFilas, 6);
                rango.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                rango.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

                // (Opcional) convertir a tabla Excel
                var table = rango.CreateTable();
                table.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium7;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ReporteTickets.xlsx");
                }
            }
        }

    }
}