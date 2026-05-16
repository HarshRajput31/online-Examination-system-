/* -----------------------------------------------------------------------
 * fullscreen.js
 *   Helpers for entering and watching fullscreen during an exam.
 *
 *   Window.ExamFullscreen exposes:
 *     enter()   - request fullscreen on document.documentElement
 *     exit()    - leave fullscreen
 *     watch(cb) - calls cb(isFullscreen:bool) on every change
 *
 *   Usage on StartExam.aspx:
 *     <button onclick="ExamFullscreen.enter()">Start in Fullscreen</button>
 *     <script>
 *       ExamFullscreen.watch(function(on) {
 *         if (!on) document.getElementById('fsExitWarning').style.display='block';
 *       });
 *     </script>
 * --------------------------------------------------------------------- */
(function (window, document) {
    'use strict';

    function isOn() {
        return !!(document.fullscreenElement ||
                  document.webkitFullscreenElement ||
                  document.mozFullScreenElement ||
                  document.msFullscreenElement);
    }

    function enter() {
        var el = document.documentElement;
        var req = el.requestFullscreen ||
                  el.webkitRequestFullscreen ||
                  el.mozRequestFullScreen ||
                  el.msRequestFullscreen;
        if (req) return req.call(el).catch(function () { /* ignore */ });
    }

    function exit() {
        var ex = document.exitFullscreen ||
                 document.webkitExitFullscreen ||
                 document.mozCancelFullScreen ||
                 document.msExitFullscreen;
        if (ex && isOn()) return ex.call(document).catch(function () { /* ignore */ });
    }

    function watch(callback) {
        if (typeof callback !== 'function') return;
        ['fullscreenchange', 'webkitfullscreenchange',
         'mozfullscreenchange', 'MSFullscreenChange']
            .forEach(function (evt) {
                document.addEventListener(evt, function () {
                    callback(isOn());
                });
            });
    }

    window.ExamFullscreen = {
        isOn: isOn,
        enter: enter,
        exit: exit,
        watch: watch
    };
})(window, document);
