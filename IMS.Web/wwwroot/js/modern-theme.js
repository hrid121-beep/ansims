// modern-theme.js - Modern React-like Interactions for ASP.NET MVC

// ===== Core Utilities =====
const ModernTheme = {
    // Animation observer for scroll animations
    initScrollAnimations() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');

                    // Stagger animations for child elements
                    const children = entry.target.querySelectorAll('.animate-child');
                    children.forEach((child, index) => {
                        setTimeout(() => {
                            child.classList.add('animate-in');
                        }, index * 100);
                    });
                }
            });
        }, observerOptions);

        // Observe all elements with animation classes
        document.querySelectorAll('.animate-on-scroll').forEach(el => {
            observer.observe(el);
        });
    },

    // Counter animations
    initCounters() {
        const counters = document.querySelectorAll('[data-counter]');

        counters.forEach(counter => {
            const target = parseFloat(counter.getAttribute('data-counter').replace(/,/g, ''));
            const duration = 2000;
            const step = target / (duration / 16);
            let current = 0;

            const updateCounter = () => {
                current += step;
                if (current < target) {
                    counter.textContent = this.formatNumber(Math.floor(current));
                    requestAnimationFrame(updateCounter);
                } else {
                    counter.textContent = this.formatNumber(target);
                }
            };

            // Start animation when element is in view
            const observer = new IntersectionObserver((entries) => {
                if (entries[0].isIntersecting) {
                    updateCounter();
                    observer.disconnect();
                }
            });

            observer.observe(counter);
        });
    },

    // Number formatting
    formatNumber(num) {
        return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    },

    // Ripple effect for buttons
    initRippleEffect() {
        document.addEventListener('click', (e) => {
            const button = e.target.closest('.btn-modern, .btn, .ripple-effect');
            if (!button) return;

            const ripple = document.createElement('span');
            const rect = button.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.cssText = `
                position: absolute;
                width: ${size}px;
                height: ${size}px;
                left: ${x}px;
                top: ${y}px;
                background: rgba(255, 255, 255, 0.5);
                border-radius: 50%;
                transform: scale(0);
                animation: ripple-animation 0.6s ease-out;
            `;

            button.style.position = 'relative';
            button.style.overflow = 'hidden';
            button.appendChild(ripple);

            setTimeout(() => ripple.remove(), 600);
        });

        // Add ripple animation to stylesheet
        if (!document.querySelector('#ripple-style')) {
            const style = document.createElement('style');
            style.id = 'ripple-style';
            style.textContent = `
                @keyframes ripple-animation {
                    to {
                        transform: scale(4);
                        opacity: 0;
                    }
                }
            `;
            document.head.appendChild(style);
        }
    },

    // Smooth page transitions
    initPageTransitions() {
        // Add transition class to body
        document.body.classList.add('page-transition-enabled');

        // Handle link clicks
        document.addEventListener('click', (e) => {
            const link = e.target.closest('a[href]:not([target="_blank"]):not([href^="#"]):not([href^="mailto"]):not([href^="tel"])');
            if (!link || link.getAttribute('data-no-transition')) return;

            const href = link.getAttribute('href');
            if (href && !href.includes('#')) {
                e.preventDefault();

                // Add exit animation
                document.body.classList.add('page-transition-exit');

                setTimeout(() => {
                    window.location.href = href;
                }, 300);
            }
        });

        // Remove exit animation on page show (for back button)
        window.addEventListener('pageshow', (e) => {
            if (e.persisted) {
                document.body.classList.remove('page-transition-exit');
            }
        });
    },

    // Modern form enhancements
    initFormEnhancements() {
        // Floating labels
        const floatingInputs = document.querySelectorAll('.form-floating-modern input, .form-floating-modern select, .form-floating-modern textarea');

        floatingInputs.forEach(input => {
            // Check initial state
            if (input.value) {
                input.classList.add('has-value');
            }

            // Handle input events
            input.addEventListener('input', () => {
                if (input.value) {
                    input.classList.add('has-value');
                } else {
                    input.classList.remove('has-value');
                }
            });

            // Handle select elements
            if (input.tagName === 'SELECT') {
                input.addEventListener('change', () => {
                    if (input.value) {
                        input.classList.add('has-value');
                    } else {
                        input.classList.remove('has-value');
                    }
                });
            }
        });

        // Input animations
        const inputs = document.querySelectorAll('input:not([type="checkbox"]):not([type="radio"]), textarea, select');
        inputs.forEach(input => {
            input.addEventListener('focus', () => {
                input.parentElement.classList.add('input-focused');
            });

            input.addEventListener('blur', () => {
                input.parentElement.classList.remove('input-focused');
            });
        });
    },

    // Toast notifications
    showToast(message, type = 'info', duration = 3000) {
        const toast = document.createElement('div');
        toast.className = `toast-modern ${type}`;

        const icons = {
            success: 'fa-check-circle',
            error: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        toast.innerHTML = `
            <div class="toast-icon">
                <i class="fas ${icons[type]}"></i>
            </div>
            <div class="toast-content">
                <div class="toast-message">${message}</div>
            </div>
            <button class="toast-close" onclick="this.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
            <div class="toast-progress"></div>
        `;

        document.body.appendChild(toast);

        // Auto remove
        setTimeout(() => {
            toast.classList.add('toast-exit');
            setTimeout(() => toast.remove(), 300);
        }, duration);
    },

    // Loading states
    showLoading(element) {
        const loader = document.createElement('div');
        loader.className = 'loading-overlay-element';
        loader.innerHTML = '<div class="spinner"></div>';

        element.style.position = 'relative';
        element.appendChild(loader);
        element.classList.add('loading');
    },

    hideLoading(element) {
        const loader = element.querySelector('.loading-overlay-element');
        if (loader) {
            loader.remove();
            element.classList.remove('loading');
        }
    },

    // Skeleton loading
    createSkeleton(type = 'text', count = 3) {
        let skeleton = '';

        switch (type) {
            case 'text':
                for (let i = 0; i < count; i++) {
                    skeleton += '<div class="skeleton-loader skeleton-text"></div>';
                }
                break;
            case 'card':
                skeleton = `
                    <div class="skeleton-loader skeleton-title"></div>
                    <div class="skeleton-loader skeleton-text"></div>
                    <div class="skeleton-loader skeleton-text"></div>
                `;
                break;
            case 'table-row':
                skeleton = '<tr>';
                for (let i = 0; i < count; i++) {
                    skeleton += '<td><div class="skeleton-loader skeleton-text"></div></td>';
                }
                skeleton += '</tr>';
                break;
        }

        return skeleton;
    },

    // Modern tooltips
    initTooltips() {
        const tooltips = document.querySelectorAll('[data-tooltip]');

        tooltips.forEach(element => {
            let tooltip = null;

            element.addEventListener('mouseenter', (e) => {
                const text = element.getAttribute('data-tooltip');
                const position = element.getAttribute('data-tooltip-position') || 'top';

                tooltip = document.createElement('div');
                tooltip.className = `tooltip-modern tooltip-${position}`;
                tooltip.textContent = text;

                document.body.appendChild(tooltip);

                // Position tooltip
                const rect = element.getBoundingClientRect();
                const tooltipRect = tooltip.getBoundingClientRect();

                switch (position) {
                    case 'top':
                        tooltip.style.left = `${rect.left + rect.width / 2 - tooltipRect.width / 2}px`;
                        tooltip.style.top = `${rect.top - tooltipRect.height - 8}px`;
                        break;
                    case 'bottom':
                        tooltip.style.left = `${rect.left + rect.width / 2 - tooltipRect.width / 2}px`;
                        tooltip.style.top = `${rect.bottom + 8}px`;
                        break;
                    case 'left':
                        tooltip.style.left = `${rect.left - tooltipRect.width - 8}px`;
                        tooltip.style.top = `${rect.top + rect.height / 2 - tooltipRect.height / 2}px`;
                        break;
                    case 'right':
                        tooltip.style.left = `${rect.right + 8}px`;
                        tooltip.style.top = `${rect.top + rect.height / 2 - tooltipRect.height / 2}px`;
                        break;
                }

                tooltip.classList.add('tooltip-show');
            });

            element.addEventListener('mouseleave', () => {
                if (tooltip) {
                    tooltip.classList.remove('tooltip-show');
                    setTimeout(() => tooltip.remove(), 300);
                }
            });
        });
    },

    // Smooth scroll
    initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));

                if (target) {
                    const offset = 80; // Account for fixed header
                    const targetPosition = target.getBoundingClientRect().top + window.pageYOffset - offset;

                    window.scrollTo({
                        top: targetPosition,
                        behavior: 'smooth'
                    });
                }
            });
        });
    },

    // Parallax effects
    initParallax() {
        const parallaxElements = document.querySelectorAll('.parallax');

        const handleParallax = () => {
            const scrolled = window.pageYOffset;

            parallaxElements.forEach(element => {
                const speed = element.getAttribute('data-speed') || 0.5;
                const yPos = -(scrolled * speed);

                element.style.transform = `translateY(${yPos}px)`;
            });
        };

        window.addEventListener('scroll', () => {
            requestAnimationFrame(handleParallax);
        });
    },

    // Card hover effects
    initCardEffects() {
        const cards = document.querySelectorAll('.card-modern, .card-3d');

        cards.forEach(card => {
            card.addEventListener('mousemove', (e) => {
                const rect = card.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;

                const centerX = rect.width / 2;
                const centerY = rect.height / 2;

                const rotateX = (y - centerY) / 10;
                const rotateY = (centerX - x) / 10;

                card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale(1.02)`;
            });

            card.addEventListener('mouseleave', () => {
                card.style.transform = 'perspective(1000px) rotateX(0) rotateY(0) scale(1)';
            });
        });
    },

    // Progress bars
    animateProgressBars() {
        const progressBars = document.querySelectorAll('.progress-bar-modern');

        progressBars.forEach(bar => {
            const target = bar.getAttribute('data-progress') || bar.style.width;

            const observer = new IntersectionObserver((entries) => {
                if (entries[0].isIntersecting) {
                    bar.style.width = target;
                    observer.disconnect();
                }
            });

            bar.style.width = '0';
            observer.observe(bar);
        });
    },

    // Modern select enhancement
    enhanceSelects() {
        const selects = document.querySelectorAll('select:not(.select2)');

        selects.forEach(select => {
            if (select.classList.contains('enhanced')) return;

            const wrapper = document.createElement('div');
            wrapper.className = 'select-modern';

            select.parentNode.insertBefore(wrapper, select);
            wrapper.appendChild(select);

            const arrow = document.createElement('i');
            arrow.className = 'fas fa-chevron-down select-arrow';
            wrapper.appendChild(arrow);

            select.classList.add('enhanced');
        });
    },

    // Activity feed updates
    addActivityItem(title, time, icon = 'fa-bell', iconColor = 'primary') {
        const feed = document.querySelector('.activity-feed');
        if (!feed) return;

        const item = document.createElement('li');
        item.className = 'activity-item';
        item.style.opacity = '0';

        item.innerHTML = `
            <div class="activity-icon bg-${iconColor}-light">
                <i class="fas ${icon} text-${iconColor}"></i>
            </div>
            <div class="activity-content">
                <div class="activity-title">${title}</div>
                <div class="activity-time">${time}</div>
            </div>
        `;

        feed.insertBefore(item, feed.firstChild);

        // Animate in
        setTimeout(() => {
            item.style.transition = 'all 0.3s ease';
            item.style.opacity = '1';
        }, 10);

        // Remove old items if too many
        const items = feed.querySelectorAll('.activity-item');
        if (items.length > 10) {
            items[items.length - 1].remove();
        }
    },

    // Initialize all features
    init() {
        this.initScrollAnimations();
        this.initCounters();
        this.initRippleEffect();
        this.initPageTransitions();
        this.initFormEnhancements();
        this.initTooltips();
        this.initSmoothScroll();
        this.initParallax();
        this.initCardEffects();
        this.animateProgressBars();
        this.enhanceSelects();

        // Add page transition styles
        this.addPageTransitionStyles();

        // Add tooltip styles
        this.addTooltipStyles();

        console.log('Modern Theme initialized successfully!');
    },

    // Add required styles dynamically
    addPageTransitionStyles() {
        if (!document.querySelector('#page-transition-styles')) {
            const style = document.createElement('style');
            style.id = 'page-transition-styles';
            style.textContent = `
                .page-transition-enabled {
                    transition: opacity 0.3s ease;
                }
                
                .page-transition-exit {
                    opacity: 0;
                }
                
                .animate-on-scroll {
                    opacity: 0;
                    transform: translateY(20px);
                    transition: all 0.6s ease;
                }
                
                .animate-on-scroll.animate-in {
                    opacity: 1;
                    transform: translateY(0);
                }
                
                .animate-child {
                    opacity: 0;
                    transform: translateY(20px);
                    transition: all 0.6s ease;
                }
                
                .animate-child.animate-in {
                    opacity: 1;
                    transform: translateY(0);
                }
                
                .loading-overlay-element {
                    position: absolute;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: rgba(255, 255, 255, 0.9);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    z-index: 10;
                }
                
                .loading-overlay-element .spinner {
                    width: 40px;
                    height: 40px;
                    border: 3px solid #f3f3f3;
                    border-top: 3px solid var(--primary);
                    border-radius: 50%;
                    animation: spin 1s linear infinite;
                }
                
                @keyframes spin {
                    0% { transform: rotate(0deg); }
                    100% { transform: rotate(360deg); }
                }
            `;
            document.head.appendChild(style);
        }
    },

    addTooltipStyles() {
        if (!document.querySelector('#tooltip-styles')) {
            const style = document.createElement('style');
            style.id = 'tooltip-styles';
            style.textContent = `
                .tooltip-modern {
                    position: fixed;
                    background: rgba(0, 0, 0, 0.9);
                    color: white;
                    padding: 0.5rem 0.75rem;
                    border-radius: 0.375rem;
                    font-size: 0.875rem;
                    z-index: 9999;
                    pointer-events: none;
                    opacity: 0;
                    transition: opacity 0.3s ease;
                }
                
                .tooltip-modern.tooltip-show {
                    opacity: 1;
                }
                
                .toast-close {
                    background: none;
                    border: none;
                    color: #64748b;
                    cursor: pointer;
                    margin-left: auto;
                    padding: 0.25rem;
                }
                
                .toast-exit {
                    animation: slideOutRight 0.3s ease-out forwards;
                }
                
                @keyframes slideOutRight {
                    to {
                        transform: translateX(100%);
                        opacity: 0;
                    }
                }
                
                .select-modern {
                    position: relative;
                    display: inline-block;
                    width: 100%;
                }
                
                .select-modern select {
                    appearance: none;
                    -webkit-appearance: none;
                    -moz-appearance: none;
                    padding-right: 2.5rem;
                }
                
                .select-modern .select-arrow {
                    position: absolute;
                    right: 1rem;
                    top: 50%;
                    transform: translateY(-50%);
                    pointer-events: none;
                    color: #64748b;
                    transition: transform 0.3s ease;
                }
                
                .select-modern select:focus ~ .select-arrow {
                    color: var(--primary);
                    transform: translateY(-50%) rotate(180deg);
                }
                
                .bg-primary-light {
                    background: rgba(99, 102, 241, 0.1);
                }
                
                .text-primary {
                    color: var(--primary);
                }
            `;
            document.head.appendChild(style);
        }
    }
};

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => ModernTheme.init());
} else {
    ModernTheme.init();
}

// Export for global use
window.ModernTheme = ModernTheme;