let auditUsersData = [];
let auditUsersPaginaActual = 1;
const auditUsersFilasPorPagina = 10;

function cargarAuditoriaUsuarios() {
    const usuarioAfectado = document.getElementById("filtroUsuarioAfectado").value;
    const accion = document.getElementById("filtroAccion").value;
    const usuarioAccion = document.getElementById("filtroUsuarioAccion").value;

    fetch(`/Admin/GetAuditUsersData?usuarioAfectado=${encodeURIComponent(usuarioAfectado)}&accion=${encodeURIComponent(accion)}&usuarioAccion=${encodeURIComponent(usuarioAccion)}`)
        .then(res => res.json())
        .then(data => {
            auditUsersData = data;
            auditUsersPaginaActual = 1;
            renderTablaAuditoriaUsuarios();
        });
}

function renderTablaAuditoriaUsuarios() {
    const tbody = document.getElementById("tablaBody");
    tbody.innerHTML = "";

    const inicio = (auditUsersPaginaActual - 1) * auditUsersFilasPorPagina;
    const fin = inicio + auditUsersFilasPorPagina;
    const datos = auditUsersData.slice(inicio, fin);

    datos.forEach(a => {
        tbody.innerHTML += `
            <tr>
                <td>${a.usuarioAfectado}</td>
                <td>${a.accion}</td>
                <td>${a.detalle}</td>
                <td>${a.usuarioAccion}</td>
                <td>${a.fecha}</td>
                <td>
                    <button class="btn-action" onclick="verDetalleUsuario(${a.id})">Ver</button>
                </td>
            </tr>
        `;
    });

    renderPaginacionAuditoriaUsuarios();
}

function renderPaginacionAuditoriaUsuarios() {
    const cont = document.getElementById("paginacion");
    cont.innerHTML = "";

    const total = Math.ceil(auditUsersData.length / auditUsersFilasPorPagina);

    for (let i = 1; i <= total; i++) {
        const btn = document.createElement("button");
        btn.className = "btn-action";
        btn.innerText = i;

        if (i === auditUsersPaginaActual) {
            btn.style.opacity = "0.6";
        }

        btn.onclick = () => {
            auditUsersPaginaActual = i;
            renderTablaAuditoriaUsuarios();
        };

        cont.appendChild(btn);
    }
}

function verDetalleUsuario(id) {
    fetch(`/Admin/AuditUserDetail?id=${id}`)
        .then(res => res.text())
        .then(html => {
            document.getElementById("contenidoModalUsuario").innerHTML = html;
            document.getElementById("modalAuditUsuario").style.display = "flex";
        });
}

function cerrarModalUsuario() {
    document.getElementById("modalAuditUsuario").style.display = "none";
    document.getElementById("contenidoModalUsuario").innerHTML = "";
}

document.addEventListener("DOMContentLoaded", cargarAuditoriaUsuarios);