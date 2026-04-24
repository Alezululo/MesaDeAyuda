let reportsPaginaActual = 1;
const reportsFilasPorPagina = 5;
let reportsDataGlobal = [];

function filtrar() {
    const fechaInicio = document.getElementById("fechaInicio").value;
    const fechaFin = document.getElementById("fechaFin").value;
    const tecnico = document.getElementById("tecnico").value;
    const estado = document.getElementById("estado").value;

    fetch(`/Reports/GetReportData?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}&tecnicoId=${tecnico}&estadoId=${estado}`)
        .then(res => res.json())
        .then(data => {
            document.getElementById("kpiTotal").innerText = data.total;
            document.getElementById("kpiAbiertos").innerText = data.abiertos;
            document.getElementById("kpiEnProceso").innerText = data.enProceso;
            document.getElementById("kpiCerrados").innerText = data.cerrados;
            document.getElementById("kpiTiempo").innerText = data.promedioRespuesta + "h";
            document.getElementById("kpiResolucion").innerText = data.promedioResolucion + "h";

            reportsDataGlobal = data.tabla;
            reportsPaginaActual = 1;

            renderTablaReports(reportsDataGlobal);
        });
}

function renderTablaReports(data) {
    const tbody = document.getElementById("tablaBody");
    tbody.innerHTML = "";

    const inicio = (reportsPaginaActual - 1) * reportsFilasPorPagina;
    const fin = inicio + reportsFilasPorPagina;
    const datosPagina = data.slice(inicio, fin);

    datosPagina.forEach(t => {
        let claseEstado = t.estado.toLowerCase().replace(" ", "");

        const fila = `
            <tr>
                <td>${t.codigo}</td>
                <td>${t.tecnico ?? 'Sin asignar'}</td>
                <td>
                    <span class="status ${claseEstado}">
                        ${t.estado}
                    </span>
                </td>
                <td>${t.fecha}</td>
                <td>${t.tiempoRespuesta ? t.tiempoRespuesta.toFixed(2) + "h" : "-"}</td>
                <td>${t.tiempoResolucion ? t.tiempoResolucion.toFixed(2) + "h" : "-"}</td>
            </tr>
        `;

        tbody.innerHTML += fila;
    });

    renderPaginacionReports(data.length);
}

function renderPaginacionReports(totalRegistros) {
    const totalPaginas = Math.ceil(totalRegistros / reportsFilasPorPagina);
    const contenedor = document.getElementById("paginacion");
    contenedor.innerHTML = "";

    for (let i = 1; i <= totalPaginas; i++) {
        const btn = document.createElement("button");
        btn.className = "btn-action";
        btn.innerText = i;

        if (i === reportsPaginaActual) {
            btn.style.opacity = "0.6";
        }

        btn.onclick = () => {
            reportsPaginaActual = i;
            renderTablaReports(reportsDataGlobal);
        };

        contenedor.appendChild(btn);
    }
}

function exportarExcel() {
    const fechaInicio = document.getElementById("fechaInicio").value;
    const fechaFin = document.getElementById("fechaFin").value;
    const tecnico = document.getElementById("tecnico").value;
    const estado = document.getElementById("estado").value;

    const url = `/Reports/ExportExcel?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}&tecnicoId=${tecnico}&estadoId=${estado}`;

    window.location.href = url;
}

document.addEventListener("DOMContentLoaded", filtrar);