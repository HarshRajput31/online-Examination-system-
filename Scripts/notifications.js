/* -----------------------------------------------------------------------
 * notifications.js
 *   Polls the server (PageMethods.GetUnreadCount) every 30s to update
 *   the navbar bell badge. The page hosting this script must:
 *     - have <asp:ScriptManager EnablePageMethods="true" />
 *     - expose a [WebMethod] static GetUnreadCount() returning {count:N}
 *     - render <span id="notifBadge" class="notif-badge">0</span>
 * --------------------------------------------------------------------- */
(function (window, document) {
    'use strict';

    function setBadge(n) {
        var el = document.getElementById('notifBadge');
        if (!el) return;
        if (!n || n <= 0) { el.style.display = 'none'; el.textContent = '0'; return; }
        el.style.display = 'inline-flex';
        el.textContent = n > 99 ? '99+' : '' + n;
    }

    function poll() {
        if (typeof PageMethods === 'undefined' ||
            typeof PageMethods.GetUnreadCount !== 'function') return;
        PageMethods.GetUnreadCount(
            function (result) {
                var data = result && result.hasOwnProperty('d') ? result.d : result;
                if (data && typeof data.count !== 'undefined') setBadge(data.count);
            },
            function (/* err */) { /* swallow */ }
        );
    }

    function init() {
        poll();
        setInterval(poll, 30000);
    }

    if (document.readyState === 'loading')
        document.addEventListener('DOMContentLoaded', init);
    else init();

    window.OEXNotifications = { refresh: poll, setBadge: setBadge };
})(window, document);
