using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Security;
using TransGGP.Application.Services;

namespace TransGGP.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index(int? anio)
        {
            if (!User.TienePermiso(Permisos.ServiciosLeer))
                return Forbid();

            var resumen = _dashboardService.Generar(anio);
            return View(resumen);
        }
    }
}
