/* -----------------------------------------------------------------------
 * dashboard.js
 *   Common helpers for Admin / Faculty / Student dashboards.
 *   - Animates numeric stat counters.
 *   - Provides Dashboard.makeLineChart / makeDoughnut for Chart.js.
 *   - Tilt / hover-glow effect on glass cards (subtle pointer-driven).
 * --------------------------------------------------------------------- */
(function (window, document) {
    'use strict';

    function animateCounter(el) {
        var target = parseFloat(el.getAttribute('data-target'));
        if (isNaN(target)) target = parseFloat(el.textContent) || 0;
        var duration = parseInt(el.getAttribute('data-duration'), 10) || 1200;
        var start = 0, t0 = null;

        function step(ts) {
            if (!t0) t0 = ts;
            var p = Math.min((ts - t0) / duration, 1);
            var eased = 1 - Math.pow(1 - p, 3);
            el.textContent = Math.round(start + (target - start) * eased).toLocaleString();
            if (p < 1) requestAnimationFrame(step);
        }
        requestAnimationFrame(step);
    }

    function applyHoverGlow(card) {
        card.addEventListener('pointermove', function (e) {
            var r = card.getBoundingClientRect();
            card.style.setProperty('--mx', ((e.clientX - r.left) / r.width * 100) + '%');
            card.style.setProperty('--my', ((e.clientY - r.top) / r.height * 100) + '%');
        });
    }

    function init() {
        document.querySelectorAll('[data-counter]').forEach(animateCounter);
        document.querySelectorAll('.glass-card, .stat-card, .ce-form-card, .exam-card')
            .forEach(applyHoverGlow);
    }

    if (document.readyState === 'loading')
        document.addEventListener('DOMContentLoaded', init);
    else init();

    // -- Public chart helpers (require Chart.js to be loaded) --
    window.Dashboard = {
        makeLineChart: function (canvasId, labels, data, opts) {
            opts = opts || {};
            var ctx = document.getElementById(canvasId);
            if (!ctx || !window.Chart) return null;
            return new Chart(ctx.getContext('2d'), {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: opts.label || 'Activity',
                        data: data,
                        borderColor: opts.color || '#38bdf8',
                        backgroundColor: opts.fillColor || 'rgba(56, 189, 248, 0.12)',
                        fill: true,
                        tension: 0.4,
                        pointRadius: 3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { beginAtZero: true, ticks: { color: '#94a3b8', stepSize: 1 },
                             grid: { color: 'rgba(255,255,255,0.06)' } },
                        x: { ticks: { color: '#94a3b8' },
                             grid: { color: 'rgba(255,255,255,0.04)' } }
                    }
                }
            });
        },

        makeDoughnut: function (canvasId, labels, data, colors) {
            var ctx = document.getElementById(canvasId);
            if (!ctx || !window.Chart) return null;
            return new Chart(ctx.getContext('2d'), {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        data: data,
                        backgroundColor: colors || ['#3b82f6', '#10b981', '#f59e0b', '#ec4899'],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { position: 'bottom', labels: { color: '#94a3b8' } } }
                }
            });
        }
    };
})(window, document);
