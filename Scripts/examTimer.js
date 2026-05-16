/* -----------------------------------------------------------------------
 * examTimer.js
 *   Countdown timer for StartExam.aspx.
 *   - Reads duration (in seconds) and a hidden submit-button selector
 *     from data attributes on a target element.
 *   - Persists the deadline in sessionStorage so refresh doesn't reset.
 *   - Auto-submits the form when time expires.
 *
 * Usage in markup:
 *   <div id="examTimer"
 *        data-duration-seconds="1800"
 *        data-exam-id="EX1234"
 *        data-submit-button="<%= btnSubmit.ClientID %>">
 *     <span id="examTimerText">30:00</span>
 *   </div>
 * --------------------------------------------------------------------- */
(function (window, document) {
    'use strict';

    function format(seconds) {
        if (seconds < 0) seconds = 0;
        var h = Math.floor(seconds / 3600);
        var m = Math.floor((seconds % 3600) / 60);
        var s = seconds % 60;
        var pad = function (n) { return n < 10 ? '0' + n : '' + n; };
        return (h > 0 ? pad(h) + ':' : '') + pad(m) + ':' + pad(s);
    }

    function init() {
        var root = document.getElementById('examTimer');
        if (!root) return;

        var label = document.getElementById('examTimerText') || root;
        var duration = parseInt(root.getAttribute('data-duration-seconds'), 10);
        var examId = root.getAttribute('data-exam-id') || 'exam';
        var submitBtnId = root.getAttribute('data-submit-button');
        if (!duration || duration <= 0) return;

        var key = 'examDeadline_' + examId;
        var deadline = parseInt(sessionStorage.getItem(key), 10);
        var now = Date.now();

        if (!deadline || isNaN(deadline) || deadline <= now) {
            deadline = now + duration * 1000;
            sessionStorage.setItem(key, deadline);
        }

        function tick() {
            var remaining = Math.floor((deadline - Date.now()) / 1000);
            label.textContent = format(remaining);

            // Warning thresholds
            if (remaining <= 60) root.classList.add('timer-critical');
            else if (remaining <= 300) root.classList.add('timer-warning');

            if (remaining <= 0) {
                clearInterval(handle);
                sessionStorage.removeItem(key);
                autoSubmit();
            }
        }

        function autoSubmit() {
            label.textContent = '00:00';
            var btn = submitBtnId ? document.getElementById(submitBtnId) : null;
            // Prefer clicking the ASP.NET submit button so __doPostBack fires.
            if (btn && typeof btn.click === 'function') { btn.click(); return; }
            // Fallback: submit the first form on the page.
            var form = document.forms[0];
            if (form) form.submit();
        }

        tick();
        var handle = setInterval(tick, 1000);

        // Expose for other scripts (e.g. anti-cheat may force-submit)
        window.ExamTimer = {
            forceSubmit: autoSubmit,
            getDeadline: function () { return deadline; }
        };
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})(window, document);
