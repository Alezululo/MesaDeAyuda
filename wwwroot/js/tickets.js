let ticketsPaginaActual = 1;
const ticketsFilasPorPagina = 10;

function verTicketAdmin(id) {
    fetch(`/Ticket/DetailsAdmin/${id}`)
        .then(res => res.text())
        .then(html => {
            document.getElementById("contenidoModal").innerHTML = html;
            document.getElementById("modalTicket").style.display = "flex";
        });
}

function cerrarModal() {
    document.getElementById("modalTicket").style.display = "none";
    document.getElementById("contenidoModal").innerHTML = "";
}

function guardarCambios(id) {
    const tecnicoId = document.getElementById("tecnicoId").value;
    const estadoId = document.getElementById("estadoId").value;
    const prioridadId = document.getElementById("prioridadId").value;
    const comentario = document.getElementById("comentario").value;

    fetch("/Ticket/UpdateTicket", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify({
            id: id,
            tecnicoId: tecnicoId || null,
            estadoId: parseInt(estadoId),
            prioridadId: parseInt(prioridadId),
            comentario: comentario
        })
    })
        .then(res => {
            if (!res.ok) throw new Error("Error al guardar");
            return res.text();
        })
        .then(() => {
            alert("Cambios guardados correctamente");
            cerrarModal();
            location.reload();
        })
        .catch(err => {
            console.error(err);
            alert("Error al guardar cambios");
        });
}

function initPaginacionTickets() {
    const filas = Array.from(document.querySelectorAll("#tablaBody tr"));

    function mostrarPagina() {
        const inicio = (ticketsPaginaActual - 1) * ticketsFilasPorPagina;
        const fin = inicio + ticketsFilasPorPagina;

        filas.forEach((fila, index) => {
            fila.style.display = (index >= inicio && index < fin) ? "" : "none";
        });

        renderPaginacionTickets();
    }

    function renderPaginacionTickets() {
        const totalPaginas = Math.ceil(filas.length / ticketsFilasPorPagina);
        const contenedor = document.getElementById("paginacion");
        contenedor.innerHTML = "";

        for (let i = 1; i <= totalPaginas; i++) {
            const btn = document.createElement("button");
            btn.className = "btn-action";
            btn.innerText = i;

            if (i === ticketsPaginaActual) {
                btn.style.opacity = "0.6";
            }

            btn.onclick = () => {
                ticketsPaginaActual = i;
                mostrarPagina();
            };

            contenedor.appendChild(btn);
        }
    }

    if (filas.length > 0) {
        mostrarPagina();
    }
}

document.addEventListener("DOMContentLoaded", initPaginacionTickets);