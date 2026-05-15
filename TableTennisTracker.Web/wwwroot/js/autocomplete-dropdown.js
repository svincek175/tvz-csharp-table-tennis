(() => {
    const controlSelector = '[data-autocomplete-control]';
    const activeClass = 'is-active';

    const escapeHtml = (value) => String(value)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#39;');

    const closeMenu = (control) => {
        const results = control.querySelector('.autocomplete-results');
        const input = control.querySelector('.autocomplete-input');
        if (results) {
            results.hidden = true;
            results.innerHTML = '';
        }
        if (input) {
            input.setAttribute('aria-expanded', 'false');
        }
        control.dataset.open = 'false';
        
        // Dispatch custom event for animations
        document.dispatchEvent(new CustomEvent('autocomplete-results-hidden', {
            detail: { container: results }
        }));
    };

    const openMenu = (control) => {
        const results = control.querySelector('.autocomplete-results');
        const input = control.querySelector('.autocomplete-input');
        if (results) {
            results.hidden = false;
        }
        if (input) {
            input.setAttribute('aria-expanded', 'true');
        }
        control.dataset.open = 'true';
        
        // Dispatch custom event for animations
        document.dispatchEvent(new CustomEvent('autocomplete-results-shown', {
            detail: { container: results }
        }));
    };

    const renderResults = (control, items) => {
        const results = control.querySelector('.autocomplete-results');
        if (!results) {
            return;
        }
        if (!items || !items.length) {
            results.innerHTML = '<div class="autocomplete-empty">No results found</div>';
            openMenu(control);
            return;
        }

        // Normalize item property names to support PascalCase or camelCase JSON
        results.innerHTML = items.map((item, index) => {
            const id = item.id ?? item.Id ?? '';
            const text = item.text ?? item.Text ?? '';
            const subtextVal = item.subtext ?? item.Subtext ?? '';
            const subtext = subtextVal ? `<span>${escapeHtml(subtextVal)}</span>` : '';
            return `
                <button type="button" class="autocomplete-option" data-value="${escapeHtml(id)}" data-text="${escapeHtml(text)}" data-index="${index}">
                    <strong>${escapeHtml(text)}</strong>
                    ${subtext}
                </button>
            `;
        }).join('');

        openMenu(control);
    };

    const fetchResults = async (control, query, abortController) => {
        const url = new URL(control.dataset.lookupUrl, window.location.origin);
        // Always include the query param (empty string allowed) so servers can return initial suggestions
        url.searchParams.set('query', query || '');

        const response = await fetch(url.toString(), {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            },
            signal: abortController.signal
        });

        if (!response.ok) {
            throw new Error(`Autocomplete request failed: ${response.status}`);
        }

        return await response.json();
    };

    const initControl = (control) => {
        if (control.dataset.autocompleteInitialized === 'true') {
            return;
        }

        control.dataset.autocompleteInitialized = 'true';
        const hiddenInput = control.querySelector('input[type="hidden"]');
        const searchInput = control.querySelector('.autocomplete-input');
        const results = control.querySelector('.autocomplete-results');
        let activeIndex = -1;
        let debounceTimer = null;
        let currentAbortController = null;

        const setSelection = (value, text) => {
            if (hiddenInput) {
                hiddenInput.value = value;
            }
            if (searchInput) {
                searchInput.value = text;
                searchInput.dataset.selectedText = text;
            }
            closeMenu(control);
        };

        const moveActive = (step) => {
            if (!results) {
                return;
            }

            const options = Array.from(results.querySelectorAll('.autocomplete-option'));
            if (!options.length) {
                return;
            }

            activeIndex = Math.max(0, Math.min(options.length - 1, activeIndex + step));
            options.forEach((option, index) => option.classList.toggle(activeClass, index === activeIndex));
            options[activeIndex]?.scrollIntoView({ block: 'nearest' });
        };

        const selectActive = () => {
            if (!results) {
                return;
            }

            const options = Array.from(results.querySelectorAll('.autocomplete-option'));
            const option = options[activeIndex];
            if (!option) {
                return;
            }

            setSelection(option.dataset.value || '', option.dataset.text || '');
        };

        const doFetch = async (query) => {
            if (currentAbortController) {
                currentAbortController.abort();
            }
            clearTimeout(debounceTimer);
            debounceTimer = window.setTimeout(async () => {
                currentAbortController = new AbortController();
                if (results) {
                    results.innerHTML = '<div class="autocomplete-loading">Searching...</div>';
                    openMenu(control);
                }

                try {
                    const items = await fetchResults(control, query, currentAbortController);
                    renderResults(control, Array.isArray(items) ? items : []);
                } catch (error) {
                    if (error.name !== 'AbortError') {
                        closeMenu(control);
                    }
                }
            }, 250);
        };

        // Immediate fetch without debounce (for toggle/open actions)
        const fetchNow = async (query) => {
            if (currentAbortController) {
                currentAbortController.abort();
            }
            clearTimeout(debounceTimer);
            currentAbortController = new AbortController();
            if (results) {
                results.innerHTML = '<div class="autocomplete-loading">Searching...</div>';
                openMenu(control);
            }
            try {
                const items = await fetchResults(control, query, currentAbortController);
                renderResults(control, Array.isArray(items) ? items : []);
            } catch (error) {
                if (error.name !== 'AbortError') {
                    closeMenu(control);
                }
            }
        };

        searchInput?.addEventListener('input', () => {
            const query = searchInput.value.trim();
            if (hiddenInput) {
                hiddenInput.value = '';
            }
            searchInput.dataset.selectedText = '';
            activeIndex = -1;

            if (!query) {
                // If the input is empty, fetch initial suggestions instead of closing
                doFetch('');
                return;
            }

            doFetch(query);
        });

        // If user clicks the rightmost area of the input (the visual caret area),
        // treat it as a toggle and open suggestions immediately.
        searchInput?.addEventListener('click', (e) => {
            try {
                const rect = searchInput.getBoundingClientRect();
                const distanceFromRight = rect.right - e.clientX;
                if (distanceFromRight >= 0 && distanceFromRight <= 56) {
                    // emulate toggle
                    fetchNow('');
                }
            } catch (_) {
                // ignore in unsupported environments
            }
        });

        searchInput?.addEventListener('focus', () => {
            if (searchInput.value.trim()) {
                searchInput.dispatchEvent(new Event('input', { bubbles: true }));
            }
        });

        searchInput?.addEventListener('keydown', (event) => {
                if (event.key === 'ArrowDown') {
                event.preventDefault();
                if (results?.hidden) {
                    // open and fetch initial items immediately
                    fetchNow('');
                    setTimeout(() => moveActive(1), 150);
                } else {
                    moveActive(1);
                }
            } else if (event.key === 'ArrowUp') {
                event.preventDefault();
                moveActive(-1);
            } else if (event.key === 'Enter') {
                event.preventDefault();
                selectActive();
            } else if (event.key === 'Escape') {
                closeMenu(control);
            }
        });

        // Toggle button support
        const toggle = control.querySelector('.autocomplete-toggle');
        toggle?.addEventListener('click', (e) => {
            e.preventDefault();
            if (control.dataset.open === 'true') {
                closeMenu(control);
                return;
            }

            // show initial suggestions immediately and focus input
            fetchNow('');
            searchInput?.focus();
        });

        results?.addEventListener('mouseover', (event) => {
            const option = event.target.closest('.autocomplete-option');
            if (!option) {
                return;
            }

            const options = Array.from(results.querySelectorAll('.autocomplete-option'));
            activeIndex = options.indexOf(option);
            options.forEach((item, index) => item.classList.toggle(activeClass, index === activeIndex));
        });

        results?.addEventListener('click', (event) => {
            const option = event.target.closest('.autocomplete-option');
            if (!option) {
                return;
            }

            setSelection(option.dataset.value || '', option.dataset.text || '');
        });

        document.addEventListener('click', (event) => {
            if (!control.contains(event.target)) {
                closeMenu(control);
            }
        });

        if (hiddenInput?.value && searchInput && !searchInput.value) {
            searchInput.value = hiddenInput.dataset.selectedText || '';
        }
    };

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll(controlSelector).forEach(initControl);
    });
})();
