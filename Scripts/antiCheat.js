/* -----------------------------------------------------------------------
 * antiCheat.js
 *   Lightweight client-side anti-cheating signals for exams.
 *
 *   - Disables right-click, copy/paste, text-selection, and Print/Save.
 *   - Counts tab-switch / focus-loss events; after a configurable
 *     threshold, force-submits via window.ExamTimer.forceSubmit().
 *   - Logs every violation into a hidden field
 *       <input type="hidden" id="hfViolations" runat="server" />
 *     so the server side can persist it with the exam result.
 *
 * Activate by placing this on a page that defines:
 *   <body data-anticheat="on" data-max-violations="3"> ... </body>
 * --------------------------------------------------------------------- */
(function (window, document) {
    'use strict';

    function init() {
        var body = document.body;
        if (!body || body.getAttribute('data-anticheat') !== 'on') return;

        var maxViolations = parseInt(body.getAttribute('data-max-violations'), 10) || 3;
        var hf = document.getElementById('hfViolations');
        var counter = 0;
        var log = [];

        function record(reason) {
            counter++;
            log.push({ t: new Date().toISOString(), reason: reason, n: counter });
            if (hf) hf.value = JSON.stringify(log);
            showWarning(reason, counter, maxViolations);

            if (counter >= maxViolations) {
                // Force submit through ExamTimer if available; fallback: submit form.
                if (window.ExamTimer && typeof window.ExamTimer.forceSubmit === 'function') {
                    window.ExamTimer.forceSubmit();
                } else if (document.forms[0]) {
                    document.forms[0].submit();
                }
            }
        }

        function showWarning(reason, n, max) {
            var bar = document.getElementById('antiCheatBar');
            if (!bar) {
                bar = document.createElement('div');
                bar.id = 'antiCheatBar';
                bar.style.cssText =
                    'position:fixed;top:0;left:0;right:0;z-index:9999;' +
                    'background:linear-gradient(90deg,#ef4444,#b91c1c);' +
                    'color:#fff;padding:10px 16px;font-weight:700;' +
                    'text-align:center;font-family:Poppins,sans-serif;' +
                    'box-shadow:0 4px 16px rgba(0,0,0,0.4);';
                document.body.appendChild(bar);
            }
            bar.textContent = '⚠ Warning ' + n + '/' + max + ': ' + reason;
            clearTimeout(bar._t);
            bar._t = setTimeout(function () { bar.style.display = 'none'; }, 4000);
            bar.style.display = 'block';
        }

        // 1. Block right-click + clipboard
        ['contextmenu', 'copy', 'cut', 'paste', 'selectstart', 'dragstart']
            .forEach(function (evt) {
                document.addEventListener(evt, function (e) {
                    e.preventDefault();
                    record('Blocked ' + evt);
                }, true);
            });

        // 2. Block common shortcuts (Ctrl+C, Ctrl+P, Ctrl+S, F12, Ctrl+Shift+I)
        document.addEventListener('keydown', function (e) {
            var k = e.key ? e.key.toLowerCase() : '';
            var ctrl = e.ctrlKey || e.metaKey;
            if (
                (ctrl && (k === 'c' || k === 'v' || k === 'x' || k === 'p' || k === 's' || k === 'u')) ||
                (e.key === 'F12') ||
                (ctrl && e.shiftKey && (k === 'i' || k === 'j' || k === 'c'))
            ) {
                e.preventDefault();
                record('Blocked shortcut ' + (ctrl ? 'Ctrl+' : '') + (e.shiftKey ? 'Shift+' : '') + e.key);
            }
        }, true);

        // 3. Tab-switch / window-blur detection
        document.addEventListener('visibilitychange', function () {
            if (document.hidden) record('Tab switched away');
        });
        window.addEventListener('blur', function () { record('Window lost focus'); });

        // 4. Detect resize (DevTools open) — soft signal only
        var lastW = window.innerWidth, lastH = window.innerHeight;
        window.addEventListener('resize', function () {
            if (Math.abs(window.innerWidth - lastW) > 200 ||
                Math.abs(window.innerHeight - lastH) > 200) {
                record('Suspicious resize');
            }
            lastW = window.innerWidth; lastH = window.innerHeight;
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})(window, document);
