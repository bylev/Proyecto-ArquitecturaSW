using TransGGP.Application.Interfaces;
using TransGGP.Domain.Models;

namespace TransGGP.Application.Reports
{

    public abstract class ReporteCreator
    {

        public abstract IReporteCreator CrearReporte();

        public string GenerarReporte(List<Cliente> clientes)
        {
            IReporteCreator reporte = CrearReporte();
            return reporte.Generar(clientes);
        }
    }

    public class ReporteTextoCreator : ReporteCreator
    {
        public override IReporteCreator CrearReporte() => new ReporteTexto();
    }

   
    public class ReporteCsvCreator : ReporteCreator
    {
        public override IReporteCreator CrearReporte() => new ReporteCsv();
    }
}
