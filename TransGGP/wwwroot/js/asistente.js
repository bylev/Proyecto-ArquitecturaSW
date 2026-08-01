(function () {
    var chat = document.getElementById("ggpChat");
    if (!chat) return;

    var toggle = document.getElementById("ggpChatToggle");
    var panel = document.getElementById("ggpChatPanel");
    var cuerpo = document.getElementById("ggpChatMensajes");
    var form = document.getElementById("ggpChatForm");
    var texto = document.getElementById("ggpChatTexto");

    var conversacion = [];
    var enviando = false;

    function tokenAntiCsrf() {
        var campo = document.querySelector('input[name="__RequestVerificationToken"]');
        return campo ? campo.value : "";
    }

    function abrir() {
        chat.classList.add("ggp-chat-abierto");
        panel.setAttribute("aria-hidden", "false");
        setTimeout(function () { texto.focus(); }, 200);
    }

    function cerrar() {
        chat.classList.remove("ggp-chat-abierto");
        panel.setAttribute("aria-hidden", "true");
    }

    toggle.addEventListener("click", function () {
        if (chat.classList.contains("ggp-chat-abierto")) cerrar(); else abrir();
    });

    function quitarIntro() {
        var intro = cuerpo.querySelector(".ggp-chat-intro");
        if (intro) intro.remove();
    }

    function agregarBurbuja(texto, clase) {
        quitarIntro();
        var div = document.createElement("div");
        div.className = "ggp-msg " + clase;
        div.textContent = texto;
        cuerpo.appendChild(div);
        cuerpo.scrollTop = cuerpo.scrollHeight;
        return div;
    }

    cuerpo.addEventListener("click", function (e) {
        var sug = e.target.closest(".ggp-sug");
        if (!sug) return;
        texto.value = sug.textContent.trim();
        form.dispatchEvent(new Event("submit", { cancelable: true }));
    });

    function agregarEscribiendo() {
        var div = document.createElement("div");
        div.className = "ggp-msg ggp-msg-bot ggp-escribiendo";
        div.innerHTML = "<span></span><span></span><span></span>";
        cuerpo.appendChild(div);
        cuerpo.scrollTop = cuerpo.scrollHeight;
        return div;
    }

    form.addEventListener("submit", function (e) {
        e.preventDefault();
        if (enviando) return;

        var pregunta = texto.value.trim();
        if (!pregunta) return;

        agregarBurbuja(pregunta, "ggp-msg-user");
        conversacion.push({ rol: "user", contenido: pregunta });
        texto.value = "";
        enviando = true;

        var escribiendo = agregarEscribiendo();

        fetch("/Asistente/Preguntar", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": tokenAntiCsrf()
            },
            body: JSON.stringify({
                anio: window.ggpAnio || null,
                mensajes: conversacion.map(function (m) {
                    return { rol: m.rol, contenido: m.contenido };
                })
            })
        })
            .then(function (r) {
                return r.json().then(function (data) {
                    return { ok: r.ok, data: data };
                });
            })
            .then(function (res) {
                escribiendo.remove();
                if (res.ok && res.data.respuesta) {
                    agregarBurbuja(res.data.respuesta, "ggp-msg-bot");
                    conversacion.push({ rol: "assistant", contenido: res.data.respuesta });
                } else {
                    var msg = (res.data && res.data.error) ? res.data.error : "No pude responder en este momento.";
                    agregarBurbuja(msg, "ggp-msg-bot ggp-msg-error");
                }
            })
            .catch(function () {
                escribiendo.remove();
                agregarBurbuja("Hubo un problema de conexión. Inténtalo de nuevo.", "ggp-msg-bot ggp-msg-error");
            })
            .finally(function () {
                enviando = false;
            });
    });
})();
