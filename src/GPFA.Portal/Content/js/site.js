(function () {
  "use strict";

  function alternarNavegacao() {
    var nav = document.getElementById("siteNav");
    var botao = document.getElementById("navToggle");
    if (!nav || !botao) return;

    var aberto = nav.classList.toggle("open");
    botao.setAttribute("aria-expanded", aberto ? "true" : "false");
    botao.setAttribute("aria-label", aberto ? "Fechar menu" : "Abrir menu");
    var uso = botao.querySelector("use");
    if (uso) uso.setAttribute("href", aberto ? "#i-x" : "#i-menu");
  }

  var navToggle = document.getElementById("navToggle");
  if (navToggle) navToggle.addEventListener("click", alternarNavegacao);

  var adminToggle = document.getElementById("adminToggle");
  var adminSidebar = document.getElementById("adminSidebar");
  if (adminToggle && adminSidebar) {
    adminToggle.addEventListener("click", function () {
      var aberto = adminSidebar.classList.toggle("open");
      adminToggle.setAttribute("aria-expanded", aberto ? "true" : "false");
    });

    document.addEventListener("click", function (evento) {
      if (window.innerWidth > 920) return;
      if (!adminSidebar.classList.contains("open")) return;
      if (adminSidebar.contains(evento.target) || adminToggle.contains(evento.target)) return;
      adminSidebar.classList.remove("open");
      adminToggle.setAttribute("aria-expanded", "false");
    });
  }

  document.addEventListener("keydown", function (evento) {
    if (evento.key !== "Escape") return;
    if (adminSidebar && adminSidebar.classList.contains("open")) {
      adminSidebar.classList.remove("open");
      if (adminToggle) {
        adminToggle.setAttribute("aria-expanded", "false");
        adminToggle.focus();
      }
    }
    var nav = document.getElementById("siteNav");
    if (nav && nav.classList.contains("open")) {
      nav.classList.remove("open");
      if (navToggle) {
        navToggle.setAttribute("aria-expanded", "false");
        navToggle.focus();
      }
    }
  });

  document.querySelectorAll("form").forEach(function (form) {
    form.addEventListener("submit", function () {
      var botoes = form.querySelectorAll(".js-submit-once");
      botoes.forEach(function (botao) {
        window.setTimeout(function () {
          botao.disabled = true;
          botao.classList.add("disabled");
        }, 10);
      });
    });
  });

  var ano = document.getElementById("anoAtual");
  if (ano) ano.textContent = new Date().getFullYear();
})();
