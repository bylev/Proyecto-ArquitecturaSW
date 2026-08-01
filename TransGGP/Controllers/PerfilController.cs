using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Exceptions;
using TransGGP.Application.Services;
using TransGGP.ViewModels;

namespace TransGGP.Web.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public PerfilController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult CambiarPassword()
        {
            return View(new CambiarPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarPassword(CambiarPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idTexto, out var id))
            {
                ModelState.AddModelError(string.Empty, "No se pudo identificar tu cuenta. Vuelve a iniciar sesión.");
                return View(modelo);
            }

            try
            {
                _usuarioService.CambiarPassword(id, modelo.PasswordActual, modelo.PasswordNueva);
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(modelo);
            }

            TempData["Exito"] = "Tu contraseña se actualizó correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
