using TransGGP.Application.Interfaces;
using TransGGP.Application.Dashboards;
using TransGGP.Domain.Models;

namespace TransGGP.Application.Reports
{

    public abstract class ReporteCreator
    {
        public abstract IReporteCreator CrearReporte();

        public ReporteArchivo GenerarReporte()
        {
            IReporteCreator reporte = CrearReporte();
            return reporte.Generar();
        }
    }

    public class ReporteClientesPdfCreator : ReporteCreator
    {
        private readonly List<Cliente> _clientes;
        private readonly byte[]? _logo;

        public ReporteClientesPdfCreator(List<Cliente> clientes, byte[]? logo)
        {
            _clientes = clientes;
            _logo = logo;
        }

        public override IReporteCreator CrearReporte() => new ReporteClientesPdf(_clientes, _logo);
    }

    public class ReporteServiciosPdfCreator : ReporteCreator
    {
        private readonly DashboardResumen _resumen;
        private readonly List<ServicioReporteFila> _servicios;
        private readonly byte[]? _logo;

        public ReporteServiciosPdfCreator(DashboardResumen resumen, List<ServicioReporteFila> servicios, byte[]? logo)
        {
            _resumen = resumen;
            _servicios = servicios;
            _logo = logo;
        }

        public override IReporteCreator CrearReporte() => new ReporteServiciosPdf(_resumen, _servicios, _logo);
    }
}
