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

        document.querySelectorAll("[data-quick-create]").forEach(btn => {
            btn.addEventListener("click", () => {
                const masterType = btn.getAttribute("data-quick-create");
                const selectEl = btn.closest(".input-group").querySelector("select");
                const appModal = document.getElementById("appModal");
                const appModalBody = document.getElementById("appModalBody");
                const appModalTitle = appModal?.querySelector(".modal-title");

                fetch(`/Products/QuickCreate?masterType=${masterType}`)
                    .then(r => {
                        if (!r.ok) throw new Error("Failed to load form");
                        return r.text();
                    })
                    .then(html => {
                        if (appModalBody) appModalBody.innerHTML = html;
                        if (appModalTitle) {
                            const label = btn.getAttribute("title") || "Quick Create";
                            appModalTitle.textContent = label.replace("Add new ", "New ");
                        }
                        const bsModal = new bootstrap.Modal(appModal);
                        bsModal.show();

                        const form = document.getElementById("quick-create-form");
                        form?.addEventListener("submit", e => {
                            e.preventDefault();
                            const submitBtn = form.querySelector('[type="submit"]');
                            submitBtn.disabled = true;
                            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Creating...';

                            const payload = {
                                masterType: parseInt(masterType),
                                name: document.getElementById("qc-name")?.value?.trim() || ""
                            };

                            if (document.getElementById("qc-code"))
                                payload.code = document.getElementById("qc-code").value.trim();
                            if (document.getElementById("qc-license"))
                                payload.licenseNumber = document.getElementById("qc-license").value.trim();
                            if (document.getElementById("qc-description"))
                                payload.description = document.getElementById("qc-description").value.trim();
                            if (document.getElementById("qc-value"))
                                payload.value = parseFloat(document.getElementById("qc-value").value) || null;
                            if (document.getElementById("qc-measurement-unit"))
                                payload.measurementUnit = document.getElementById("qc-measurement-unit").value.trim();
                            if (document.getElementById("qc-symbol"))
                                payload.symbol = document.getElementById("qc-symbol").value.trim();
                            if (document.getElementById("qc-allows-decimal"))
                                payload.allowsDecimal = document.getElementById("qc-allows-decimal").checked;

                            if (!payload.name) {
                                submitBtn.disabled = false;
                                submitBtn.innerHTML = '<i class="ti ti-plus me-1"></i>Create';
                                const errDiv = document.getElementById("quick-create-errors");
                                if (errDiv) {
                                    errDiv.textContent = "Name is required.";
                                    errDiv.classList.remove("d-none");
                                }
                                return;
                            }

                            fetch("/Products/QuickCreate", {
                                method: "POST",
                                headers: { "Content-Type": "application/json" },
                                body: JSON.stringify(payload)
                            })
                            .then(r => r.json())
                            .then(data => {
                                if (data.success && data.data) {
                                    const opt = document.createElement("option");
                                    opt.value = data.data.id;
                                    opt.textContent = data.data.name;
                                    selectEl.appendChild(opt);
                                    selectEl.value = data.data.id;
                                    bsModal.hide();
                                    if (typeof common !== "undefined") common.successToast("Created successfully.");
                                } else {
                                    submitBtn.disabled = false;
                                    submitBtn.innerHTML = '<i class="ti ti-plus me-1"></i>Create';
                                    const errDiv = document.getElementById("quick-create-errors");
                                    if (errDiv) {
                                        errDiv.textContent = data.message || "Failed to create.";
                                        errDiv.classList.remove("d-none");
                                    }
                                }
                            })
                            .catch(() => {
                                submitBtn.disabled = false;
                                submitBtn.innerHTML = '<i class="ti ti-plus me-1"></i>Create';
                                if (typeof common !== "undefined") common.dangerToast("An error occurred.");
                            });
                        });
                    })
                    .catch(() => {
                        if (typeof common !== "undefined") common.dangerToast("Failed to load form.");
                    });
            });
        });
    });
})();
