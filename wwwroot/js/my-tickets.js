let myTicketsPaginaActual = 1;
const myTicketsFilasPorPagina = 10;

function verTicket(id) {
    fetch('/Ticket/Details?id=' + id)
        .then(response => response.text())
        .then(html => {
            document.getElementById('contenidoModal').innerHTML = html;
            document.getElementById('modalTicket').style.display = 'flex';
        });
}

function cerrarModal() {
    document.getElementById('modalTicket').style.display = 'none';
    document.getElementById('contenidoModal').innerHTML = '';
}

function initPaginacionMyTickets() {
    const filas = Array.from(document.querySelectorAll("#tablaBody tr"));

    function mostrarPagina() {
        const inicio = (myTicketsPaginaActual - 1) * myTicketsFilasPorPagina;
        const fin = inicio + myTicketsFilasPorPagina;

        filas.forEach((fila, index) => {
            fila.style.display = (index >= inicio && index < fin) ? "" : "none";
        });

        renderPaginacionMyTickets();
    }

    function renderPaginacionMyTickets() {
        const totalPaginas = Math.ceil(filas.length / myTicketsFilasPorPagina);
        const contenedor = document.getElementById("paginacion");
        contenedor.innerHTML = "";

        for (let i = 1; i <= totalPaginas; i++) {
            const btn = document.createElement("button");
            btn.className = "btn-action";
            btn.innerText = i;

            if (i === myTicketsPaginaActual) {
                btn.style.opacity = "0.6";
            }

            btn.onclick = () => {
                myTicketsPaginaActual = i;
                mostrarPagina();
            };

            contenedor.appendChild(btn);
        }
    }

    if (filas.length > 0) {
        mostrarPagina();
    }
}

document.addEventListener("DOMContentLoaded", initPaginacionMyTickets);