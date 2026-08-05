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

        if (selector) {
            selector.value = getStoredTheme();
            selector.addEventListener("change", event => {
                const theme = event.target.value === "dark" ? "dark" : "light";

                localStorage.setItem(storageKey, theme);
                applyTheme(theme);
            });
        }

        const productType = document.getElementById("ProductType");
        const medicineFields = document.querySelectorAll(".medicine-fields");

        if (productType && medicineFields.length > 0) {
            const applyProductType = () => {
                const isMedicine = productType.value === "Medicine" || productType.value === "1";

                medicineFields.forEach(field => {
                    field.classList.toggle("d-none", !isMedicine);

                    if (!isMedicine) {
                        field.querySelectorAll("input, select, textarea").forEach(input => {
                            if (input.type === "checkbox") {
                                input.checked = false;
                            } else {
                                input.value = "";
                            }
                        });
                    }
                });
            };

            productType.addEventListener("change", applyProductType);
            applyProductType();
        }
    });
})();
