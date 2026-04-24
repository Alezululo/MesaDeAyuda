let usersPaginaActual = 1;
const usersFilasPorPagina = 10;

function abrirCrearUsuario() {
    fetch('/UserManagement/Crear')
        .then(r => {
            if (!r.ok) throw new Error("Error al cargar modal");
            return r.text();
        })
        .then(html => {
            document.getElementById("contenidoModalUsuario").innerHTML = html;
            document.getElementById("modalUsuario").style.display = "flex";
        })
        .catch(err => console.error(err));
}

function editarUsuario(id) {
    fetch('/UserManagement/Editar/' + id)
        .then(r => {
            if (!r.ok) throw new Error("Error al cargar modal");
            return r.text();
        })
        .then(html => {
            document.getElementById("contenidoModalUsuario").innerHTML = html;
            document.getElementById("modalUsuario").style.display = "flex";
        })
        .catch(err => console.error(err));
}

function cerrarModalUsuario() {
    document.getElementById("modalUsuario").style.display = "none";
    document.getElementById("contenidoModalUsuario").innerHTML = "";
}

function crearUsuario() {
    const formData = new FormData();

    formData.append("nombre", document.getElementById("nombre").value);
    formData.append("username", document.getElementById("username").value);
    formData.append("password", document.getElementById("password")?.value);
    formData.append("rolId", document.getElementById("rol").value);

    const foto = document.getElementById("foto")?.files[0];
    if (foto) formData.append("foto", foto);

    fetch('/UserManagement/CrearUsuario', {
        method: 'POST',
        body: formData
    })
        .then(r => {
            if (!r.ok) throw new Error();
            location.reload();
        })
        .catch(() => alert("Error al crear usuario"));
}

function actualizarUsuario() {
    const formData = new FormData();

    formData.append("Id", document.getElementById("id").value);
    formData.append("Nombre", document.getElementById("nombre").value);
    formData.append("Username", document.getElementById("username").value);
    formData.append("RolId", document.getElementById("rol").value);
    formData.append("Activo", document.getElementById("activo").value);

    const password = document.getElementById("password")?.value;
    if (password) formData.append("password", password);

    const foto = document.getElementById("foto")?.files[0];
    if (foto) formData.append("Foto", foto);

    fetch('/UserManagement/EditarUsuario', {
        method: 'POST',
        body: formData
    })
        .then(r => {
            if (!r.ok) throw new Error();
            location.reload();
        })
        .catch(() => alert("Error al actualizar usuario"));
}

function initPaginacionUsuarios() {
    const filas = Array.from(document.querySelectorAll("#tablaBody tr"));

    function mostrarPagina() {
        const inicio = (usersPaginaActual - 1) * usersFilasPorPagina;
        const fin = inicio + usersFilasPorPagina;

        filas.forEach((fila, index) => {
            fila.style.display = (index >= inicio && index < fin) ? "" : "none";
        });

        renderPaginacionUsuarios();
    }

    function renderPaginacionUsuarios() {
        const totalPaginas = Math.ceil(filas.length / usersFilasPorPagina);
        const contenedor = document.getElementById("paginacion");
        contenedor.innerHTML = "";

        for (let i = 1; i <= totalPaginas; i++) {
            const btn = document.createElement("button");
            btn.className = "btn-action";
            btn.innerText = i;

            if (i === usersPaginaActual) {
                btn.style.opacity = "0.6";
            }

            btn.onclick = () => {
                usersPaginaActual = i;
                mostrarPagina();
            };

            contenedor.appendChild(btn);
        }
    }

    if (filas.length > 0) {
        mostrarPagina();
    }
}

document.addEventListener("DOMContentLoaded", initPaginacionUsuarios);