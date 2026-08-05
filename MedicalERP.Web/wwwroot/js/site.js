(() => {
    const storageKey = "medicalerp-theme";

    function getStoredTheme() {
        const storedTheme = localStorage.getItem(storageKey);

        return storedTheme === "dark" ? "dark" : "light";
    }

    function setStylesheet(linkId, theme) {
        const link = document.getElementById(linkId);

        if (!link) return;

        const href = theme === "dark"
            ? link.dataset.darkHref
            : link.dataset.lightHref;

        if (href && link.getAttribute("href") !== href) {
            link.setAttribute("href", href);
        }
    }

    function applyTheme(theme) {
        const root = document.documentElement;
        const isDark = theme === "dark";

        root.classList.toggle("dark-style", isDark);
        root.classList.toggle("light-style", !isDark);
        root.setAttribute("data-bs-theme", theme);

        setStylesheet("template-core-css", theme);
        setStylesheet("template-theme-css", theme);

        const selector = document.getElementById("theme-selector");

        if (selector) {
            selector.value = theme;
        }
    }

    applyTheme(getStoredTheme());

    document.addEventListener("DOMContentLoaded", () => {
        const selector = document.getElementById("theme-selector");

        if (!selector) return;

        selector.value = getStoredTheme();
        selector.addEventListener("change", event => {
            const theme = event.target.value === "dark" ? "dark" : "light";

            localStorage.setItem(storageKey, theme);
            applyTheme(theme);
        });
    });
})();
