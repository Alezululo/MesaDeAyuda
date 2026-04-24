document.addEventListener("DOMContentLoaded", () => {
    const savedTheme = localStorage.getItem("theme");
    const themeToggle = document.querySelector(".theme-switch input");

    if (savedTheme === "light") {
        document.body.classList.add("light-mode");
        if (themeToggle) {
            themeToggle.checked = true;
        }
    }
});

function toggleTheme() {
    document.body.classList.toggle("light-mode");

    const isLight = document.body.classList.contains("light-mode");
    localStorage.setItem("theme", isLight ? "light" : "dark");
}