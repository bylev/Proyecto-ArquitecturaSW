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
