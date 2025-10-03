function toggleTheme() {
    const root = document.documentElement;
    const currentTheme = root.getAttribute("data-theme");
    const newTheme = currentTheme === "dark" ? "light" : "dark";

    root.setAttribute("data-theme", newTheme);
    localStorage.setItem("theme", newTheme);

    // đổi icon nút
    const icon = document.getElementById("themeIcon");
    if (icon) {
        icon.textContent = newTheme === "dark" ? "☀️" : "🌙";
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const btn = document.getElementById("themeToggle");
    if (btn) {
        btn.addEventListener("click", toggleTheme);

        // load icon đúng theme ban đầu
        const currentTheme = document.documentElement.getAttribute("data-theme");
        const icon = document.getElementById("themeIcon");
        if (icon) {
            icon.textContent = currentTheme === "dark" ? "☀️" : "🌙";
        }
    }
});
