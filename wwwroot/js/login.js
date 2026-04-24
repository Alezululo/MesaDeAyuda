let timeoutId = null;

function mostrarMensajeCuenta() {
    mostrarAlerta("alertaCuenta", 4500);
}

function mostrarAlerta(id, tiempo) {
    const alerta = document.getElementById(id);
    if (!alerta) return;

    alerta.classList.add("show");

    if (timeoutId) clearTimeout(timeoutId);

    timeoutId = setTimeout(() => {
        if (!alerta.matches(":hover")) {
            alerta.classList.remove("show");
        }
    }, tiempo);

    alerta.onmouseleave = () => {
        alerta.classList.remove("show");
    };
}

function cerrarAlerta(id) {
    const alerta = document.getElementById(id);
    if (alerta) {
        alerta.classList.remove("show");
    }
}