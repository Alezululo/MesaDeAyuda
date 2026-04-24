let auditTicketsData = [];
let auditTicketsPaginaActual = 1;
const auditTicketsFilasPorPagina = 10;

function cargarAuditoriaTickets() {
    const ticket = document.getElementById("filtroTicket").value;
    const tipo = document.getElementById("filtroTipo").value;
    const usuario = document.getElementById("filtroUsuario").value;

    fetch(`/Admin/GetAuditTicketsData?ticket=${encodeURIComponent(ticket)}&tipo=${encodeURIComponent(tipo)}&usuario=${encodeURIComponent(usuario)}`)
        .then(res => res.json())
        .then(data => {
            auditTicketsData = data;
            auditTicketsPaginaActual = 1;
            renderTablaAuditoriaTickets();
        });
}

function renderTablaAuditoriaTickets() {
    const tbody = document.getElementById("tablaBody");
    tbody.innerHTML = "";

    const inicio = (auditTicketsPaginaActual - 1) * auditTicketsFilasPorPagina;
    const fin = inicio + auditTicketsFilasPorPagina;
    const datos = auditTicketsData.slice(inicio, fin);

    datos.forEach(a => {
        tbody.innerHTML += `
            <tr>
                <td>${a.ticket}</td>
                <td>${a.cambioDe}</td>
                <td>${a.detalle}</td>
                <td>${a.usuario}</td>
                <td>${a.fecha}</td>
                <td>
                    <button class="btn-action" onclick="verDetalleTicket(${a.id}, '${a.cambioDe}')">Ver</button>
                </td>
            </tr>
        `;
    });

    renderPaginacionAuditoriaTickets();
}

function renderPaginacionAuditoriaTickets() {
    const cont = document.getElementById("paginacion");
    cont.innerHTML = "";

    const total = Math.ceil(auditTicketsData.length / auditTicketsFilasPorPagina);

    for (let i = 1; i <= total; i++) {
        const btn = document.createElement("button");
        btn.className = "btn-action";
        btn.innerText = i;

        if (i === auditTicketsPaginaActual) btn.style.opacity = "0.6";

        btn.onclick = () => {
            auditTicketsPaginaActual = i;
            renderTablaAuditoriaTickets();
        };

        cont.appendChild(btn);
    }
}

function verDetalleTicket(id, tipo) {
    fetch(`/Admin/AuditTicketDetail?id=${id}&tipo=${encodeURIComponent(tipo)}`)
        .then(res => res.text())
        .then(html => {
            document.getElementById("contenidoModalTicket").innerHTML = html;
            document.getElementById("modalAuditTicket").style.display = "flex";
        });
}

function cerrarModalTicket() {
    document.getElementById("modalAuditTicket").style.display = "none";
    document.getElementById("contenidoModalTicket").innerHTML = "";
}

document.addEventListener("DOMContentLoaded", cargarAuditoriaTickets);