using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Reports;
using TransGGP.Application.Security;
using TransGGP.Application.Services;

namespace TransGGP.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly IWebHostEnvironment _entorno;

        public DashboardController(DashboardService dashboardService, IWebHostEnvironment entorno)
        {
            _dashboardService = dashboardService;
            _entorno = entorno;
        }

        public IActionResult Index(int? anio)
        {
            if (!User.TienePermiso(Permisos.ServiciosLeer))
                return Forbid();

            var resumen = _dashboardService.Generar(anio);
            return View(resumen);
        }

        public IActionResult Reporte(int? anio)
        {
            if (!User.TienePermiso(Permisos.ServiciosLeer))
                return Forbid();

            var resumen = _dashboardService.Generar(anio);
            var servicios = _dashboardService.ObtenerServiciosDelAnio(resumen.Anio);

            byte[]? logo = null;
            var rutaLogo = Path.Combine(_entorno.WebRootPath, "images", "logo.png");
            if (System.IO.File.Exists(rutaLogo))
                logo = System.IO.File.ReadAllBytes(rutaLogo);

            ReporteCreator creator = new ReporteServiciosPdfCreator(resumen, servicios, logo);
            ReporteArchivo archivo = creator.GenerarReporte();
            return File(archivo.Contenido, archivo.TipoContenido, archivo.NombreArchivo);
        }
    }
}
