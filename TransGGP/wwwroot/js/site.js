// Sombra de la barra al desplazar
(function () {
    var nav = document.querySelector('.ggp-navbar');
    if (!nav) return;
    function alScroll() {
        if (window.scrollY > 8) nav.classList.add('ggp-navbar-scroll');
        else nav.classList.remove('ggp-navbar-scroll');
    }
    alScroll();
    window.addEventListener('scroll', alScroll, { passive: true });
})();

// Las ventanas emergentes deben colgar del <body>.
// Si quedan dentro del contenido, la capa oscura las tapa y no se puede interactuar.
(function () {
    var modales = document.querySelectorAll('.modal');
    for (var i = 0; i < modales.length; i++) {
        if (modales[i].parentNode !== document.body) {
            document.body.appendChild(modales[i]);
        }
    }
})();

// Revelado al hacer scroll + números que cuentan
(function () {
    function contar(ambito) {
        var nums = ambito.querySelectorAll('[data-contador]:not([data-listo])');
        for (var i = 0; i < nums.length; i++) {
            (function (el) {
                el.setAttribute('data-listo', '1');
                var destino = parseInt(el.getAttribute('data-contador'), 10);
                if (isNaN(destino)) return;
                var inicio = null, dur = 1400;
                el.textContent = '0';
                function paso(t) {
                    if (!inicio) inicio = t;
                    var p = Math.min((t - inicio) / dur, 1);
                    el.textContent = Math.round(destino * (1 - Math.pow(1 - p, 3)));
                    if (p < 1) requestAnimationFrame(paso); else el.textContent = destino;
                }
                requestAnimationFrame(paso);
            })(nums[i]);
        }
    }

    var reveals = document.querySelectorAll('.rv');
    if ('IntersectionObserver' in window && reveals.length) {
        var obs = new IntersectionObserver(function (entradas) {
            entradas.forEach(function (e) {
                if (e.isIntersecting) {
                    e.target.classList.add('in');
                    contar(e.target);
                    obs.unobserve(e.target);
                }
            });
        }, { threshold: 0.12 });
        for (var i = 0; i < reveals.length; i++) obs.observe(reveals[i]);
    } else {
        for (var j = 0; j < reveals.length; j++) reveals[j].classList.add('in');
    }

    contar(document);
})();
