/* -----------------------------------------------------------------------
 * validation.js
 *   Reusable client-side form validation that complements ASP.NET
 *   validators (which fail when the form is missing one).
 *
 *   Usage:
 *     <input data-validate="required|email" ...>
 *     <input data-validate="required|min:6"   ...>
 *     <input data-validate="required|number"  ...>
 *
 *   Forms with class="js-validate" are validated on submit.
 * --------------------------------------------------------------------- */
(function (window, document) {
    'use strict';

    var rules = {
        required: function (v) { return v.trim().length > 0; },
        email:    function (v) { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v); },
        number:   function (v) { return /^\d+(\.\d+)?$/.test(v); },
        min:      function (v, n) { return v.length >= parseInt(n, 10); },
        max:      function (v, n) { return v.length <= parseInt(n, 10); }
    };

    function showError(input, msg) {
        input.classList.add('is-invalid');
        var hint = input.nextElementSibling;
        if (!hint || !hint.classList || !hint.classList.contains('field-error')) {
            hint = document.createElement('span');
            hint.className = 'field-error';
            hint.style.cssText = 'display:block;color:#fca5a5;font-size:12px;margin-top:4px;';
            input.parentNode.insertBefore(hint, input.nextSibling);
        }
        hint.textContent = msg;
    }

    function clearError(input) {
        input.classList.remove('is-invalid');
        var hint = input.nextElementSibling;
        if (hint && hint.classList && hint.classList.contains('field-error')) {
            hint.textContent = '';
        }
    }

    function validateInput(input) {
        var spec = (input.getAttribute('data-validate') || '').trim();
        if (!spec) return true;
        var v = input.value || '';
        var parts = spec.split('|');
        for (var i = 0; i < parts.length; i++) {
            var p = parts[i].split(':');
            var rule = p[0], arg = p[1];
            if (!rules[rule]) continue;
            if (!rules[rule](v, arg)) {
                var msgs = {
                    required: 'This field is required',
                    email:    'Enter a valid email',
                    number:   'Numbers only',
                    min:      'Minimum ' + arg + ' characters',
                    max:      'Maximum ' + arg + ' characters'
                };
                showError(input, msgs[rule] || 'Invalid');
                return false;
            }
        }
        clearError(input);
        return true;
    }

    function bind() {
        document.querySelectorAll('form.js-validate').forEach(function (form) {
            form.addEventListener('submit', function (e) {
                var ok = true;
                form.querySelectorAll('[data-validate]').forEach(function (input) {
                    if (!validateInput(input)) ok = false;
                });
                if (!ok) { e.preventDefault(); e.stopImmediatePropagation(); }
            });
            form.querySelectorAll('[data-validate]').forEach(function (input) {
                input.addEventListener('blur', function () { validateInput(input); });
            });
        });
    }

    if (document.readyState === 'loading')
        document.addEventListener('DOMContentLoaded', bind);
    else bind();

    window.OEXValidate = { validateInput: validateInput };
})(window, document);
