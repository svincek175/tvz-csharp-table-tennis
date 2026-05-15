(function () {
    // Simple form validation glue: validate on blur and show messages in existing validation spans
    const showMessage = (fieldName, message) => {
        const selector = `[data-valmsg-for="${fieldName}"]`;
        const span = document.querySelector(selector);
        if (!span) return;
        span.textContent = message;
        span.classList.remove('field-validation-valid');
        span.classList.add('validation-error', 'field-validation-error');
    };

    const clearMessage = (fieldName) => {
        const selector = `[data-valmsg-for="${fieldName}"]`;
        const span = document.querySelector(selector);
        if (!span) return;
        span.textContent = '';
        span.classList.remove('field-validation-error', 'validation-error');
        span.classList.add('field-validation-valid');
    };

    const validateAutocomplete = (hiddenInput) => {
        const name = hiddenInput.getAttribute('name');
        const msg = hiddenInput.dataset.requiredMessage || 'This field is required';
        if (!hiddenInput.value || hiddenInput.value === '00000000-0000-0000-0000-000000000000') {
            showMessage(name, msg);
            return false;
        }
        clearMessage(name);
        return true;
    };

    const validateRequiredHtml = (input) => {
        const name = input.getAttribute('name') || input.id;
        const required = input.required;
        if (!required) {
            clearMessage(name);
            return true;
        }

        if (!input.value || input.value.trim() === '') {
            const msg = input.getAttribute('data-required-message') || `${(input.dataset && input.dataset.label) || 'This field'} is required`;
            showMessage(name, msg);
            return false;
        }

        clearMessage(name);
        return true;
    };

    const bind = () => {
        // autocomplete hidden inputs
        document.querySelectorAll('.autocomplete-control input[type=hidden]').forEach(hidden => {
            // when the visible input loses focus, validate the hidden one
            const control = hidden.closest('.autocomplete-control');
            if (!control) return;
            const search = control.querySelector('.autocomplete-input');
            if (!search) return;

            search.addEventListener('blur', () => {
                validateAutocomplete(hidden);
            });

            // also validate when the hidden input changes programmatically
            hidden.addEventListener('change', () => {
                validateAutocomplete(hidden);
            });
        });

        // generic required HTML5 inputs
        document.querySelectorAll('input[required], textarea[required], select[required]').forEach(input => {
            input.addEventListener('blur', () => validateRequiredHtml(input));
        });

        // Run a quick validation on submit to ensure messages show
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener('submit', (e) => {
                let ok = true;
                // validate hidden autocompletes
                form.querySelectorAll('.autocomplete-control input[type=hidden]').forEach(h => {
                    if (!validateAutocomplete(h)) ok = false;
                });
                // validate HTML required
                form.querySelectorAll('input[required], textarea[required], select[required]').forEach(inp => {
                    if (!validateRequiredHtml(inp)) ok = false;
                });

                if (!ok) {
                    e.preventDefault();
                    const firstErr = form.querySelector('.field-validation-error');
                    if (firstErr) {
                        const name = firstErr.getAttribute('data-valmsg-for');
                        const field = form.querySelector(`[name="${name}"]`);
                        if (field && field.focus) field.focus();
                    }
                }
            });
        });
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', bind);
    } else {
        bind();
    }
})();
