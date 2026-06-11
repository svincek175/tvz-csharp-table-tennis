/**
 * Table Tennis Tracker - Advanced Animations
 * Demonstrates modern JavaScript techniques: Intersection Observer, RequestAnimationFrame,
 * Event Delegation, Staggered Animations, and CSS State Management
 */

class AnimationController {
    constructor() {
        this.setupFormAnimations();
        this.setupAutocompleteAnimations();
        this.setupValidationAnimations();
        this.setupButtonAnimations();
        this.setupScrollRevealAnimations();
    }

    /**
     * Form field entrance animations with staggered timing
     */
    setupFormAnimations() {
        const formGroups = document.querySelectorAll('.form-group');
        if (!formGroups.length) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting && !entry.target.classList.contains('form-group-animated')) {
                    // Calculate delay based on element position
                    const index = Array.from(formGroups).indexOf(entry.target);
                    const delay = index * 50; // 50ms stagger between fields

                    setTimeout(() => {
                        entry.target.classList.add('form-group-animated');
                        entry.target.style.animation = `slideInUp 0.6s cubic-bezier(0.34, 1.56, 0.64, 1) forwards`;
                    }, delay);

                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });

        formGroups.forEach(group => {
            // Don't hide the group initially - just animate it
            observer.observe(group);
        });
    }

    /**
     * Autocomplete dropdown animations with smooth appearance
     */
    setupAutocompleteAnimations() {
        document.addEventListener('autocomplete-results-shown', (event) => {
            const resultsContainer = event.detail?.container;
            if (!resultsContainer) return;

            resultsContainer.style.opacity = '0';
            resultsContainer.style.transform = 'translateY(-10px)';

            // Use requestAnimationFrame for smooth animation
            requestAnimationFrame(() => {
                resultsContainer.style.transition = 'opacity 0.3s ease, transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1)';
                resultsContainer.style.opacity = '1';
                resultsContainer.style.transform = 'translateY(0)';

                // Stagger animate list items
                const items = resultsContainer.querySelectorAll('.autocomplete-option, [data-result-item]');
                items.forEach((item, index) => {
                    item.style.opacity = '0';
                    item.style.transform = 'translateX(-20px)';
                    item.style.transition = `opacity 0.2s ease ${index * 30}ms, transform 0.2s ease ${index * 30}ms`;

                    // Trigger animation with setTimeout to ensure style application
                    setTimeout(() => {
                        item.style.opacity = '1';
                        item.style.transform = 'translateX(0)';
                    }, 10);
                });
            });
        });

        document.addEventListener('autocomplete-results-hidden', (event) => {
            const resultsContainer = event.detail?.container;
            if (!resultsContainer) return;

            resultsContainer.style.transition = 'opacity 0.2s ease, transform 0.2s ease';
            resultsContainer.style.opacity = '0';
            resultsContainer.style.transform = 'translateY(-10px)';
        });
    }

    /**
     * Validation error animations with shake effect and smooth appearance
     */
    setupValidationAnimations() {
        // Only observe actual validation message additions
        const errorObserver = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                // Check if validation errors were actually added
                Array.from(mutation.addedNodes).forEach((node) => {
                    if (node.nodeType === 1 && node.classList && node.classList.contains('validation-error')) {
                        if (!node.classList.contains('animated')) {
                            node.classList.add('animated');
                            node.style.animation = 'slideInError 0.4s cubic-bezier(0.34, 1.56, 0.64, 1) forwards';

                            // Add shake effect on parent form-group
                            const formGroup = node.closest('.form-group');
                            if (formGroup) {
                                formGroup.style.animation = 'shakeField 0.5s ease-in-out';
                            }
                        }
                    }
                });
            });
        });

        // Only observe specific form areas, not the entire document
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            errorObserver.observe(form, {
                childList: true,
                subtree: true,
                characterData: false,
            });
        });
    }

    /**
     * Button animations: ripple effect on click, scale on hover, loading state
     */
    setupButtonAnimations() {
        const buttons = document.querySelectorAll('.btn-submit, .btn-cancel, button[type="submit"], button[type="button"]');

        buttons.forEach((button) => {
            // Hover effect
            button.addEventListener('mouseenter', () => {
                button.style.transform = 'translateY(-2px)';
                button.style.boxShadow = '0 8px 16px rgba(0, 0, 0, 0.2)';
            });

            button.addEventListener('mouseleave', () => {
                button.style.transform = 'translateY(0)';
                button.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.1)';
            });

            // Click ripple effect
            button.addEventListener('click', (e) => {
                this.createRipple(e, button);
            });

            // Add smooth transitions
            button.style.transition = 'all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1)';
        });

        document.querySelectorAll('form').forEach((form) => {
            form.addEventListener('submit', (event) => {
                if (event.defaultPrevented) {
                    return;
                }

                const submitButton = form.querySelector('button[type="submit"]');
                if (!submitButton || submitButton.disabled) {
                    return;
                }

                submitButton.classList.add('button-submitting');
                submitButton.disabled = true;
                submitButton.style.opacity = '0.7';
            });
        });
    }

    /**
     * Ripple effect animation on button click
     */
    createRipple(event, button) {
        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = event.clientX - rect.left - size / 2;
        const y = event.clientY - rect.top - size / 2;

        ripple.style.width = ripple.style.height = size + 'px';
        ripple.style.left = x + 'px';
        ripple.style.top = y + 'px';
        ripple.classList.add('ripple');

        // Remove old ripples
        const oldRipple = button.querySelector('.ripple');
        if (oldRipple) oldRipple.remove();

        button.appendChild(ripple);

        // Remove ripple after animation
        setTimeout(() => ripple.remove(), 600);
    }

    /**
     * Scroll reveal animations for elements using Intersection Observer
     */
    setupScrollRevealAnimations() {
        const revealElements = document.querySelectorAll('.form-container, .form-header, h1, h2, h3');

        const revealObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('reveal-visible');
                    revealObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });

        revealElements.forEach(element => {
            element.classList.add('reveal');
            revealObserver.observe(element);
        });
    }

    /**
     * Focus animations for inputs
     */
    setupFocusAnimations() {
        const inputs = document.querySelectorAll('input[type="text"], input[type="email"], input[type="password"], input[type="number"], textarea, select');

        inputs.forEach((input) => {
            input.addEventListener('focus', () => {
                // Only add animation, don't modify display properties
                input.style.animation = 'focusPulse 0.4s ease-out 1';
            });

            input.addEventListener('blur', () => {
                input.style.animation = 'none';
            });
        });
    }

    /**
     * Loading animation for form submission
     */
    animateFormSubmission(form) {
        return new Promise((resolve) => {
            const button = form.querySelector('button[type="submit"]');
            if (!button) {
                resolve();
                return;
            }

            const originalText = button.textContent;
            button.textContent = '⏳ Processing...';
            button.style.animation = 'pulse 1.5s infinite';

            setTimeout(() => {
                button.style.animation = 'none';
                button.textContent = originalText;
                resolve();
            }, 800);
        });
    }

    /**
     * Success animation with checkmark
     */
    animateSuccess(container) {
        const successDiv = document.createElement('div');
        successDiv.className = 'success-message';
        successDiv.innerHTML = '✅ Success!';
        successDiv.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #10b981;
            color: white;
            padding: 16px 24px;
            border-radius: 8px;
            font-weight: 500;
            z-index: 1000;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
        `;

        document.body.appendChild(successDiv);

        successDiv.style.animation = 'slideInRight 0.5s cubic-bezier(0.34, 1.56, 0.64, 1)';

        setTimeout(() => {
            successDiv.style.animation = 'slideOutRight 0.5s cubic-bezier(0.34, 1.56, 0.64, 1) forwards';
            setTimeout(() => successDiv.remove(), 500);
        }, 3000);
    }
}

// Initialize animations when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        new AnimationController();
    });
} else {
    new AnimationController();
}
