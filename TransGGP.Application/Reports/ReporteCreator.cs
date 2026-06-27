using TransGGP.Domain.Models;

namespace TransGGP.Application.Reports
{
    /// <summary>
    /// PATRÓN FACTORY METHOD (GOF - Creacional).
    ///
    /// CREATOR abstracto. Declara el "factory method" CrearReporte(), pero NO
    /// decide qué producto concreto se crea: eso lo deciden las subclases.
    /// Así, agregar un nuevo formato de reporte no obliga a tocar este código.
    /// </summary>
    public abstract class ReporteCreator
    {
        // EL FACTORY METHOD: cada subclase decide qué tipo de reporte fabricar.
        public abstract IReporte CrearReporte();

        // Lógica común: usa el producto creado por la subclase, sin saber cuál es.
        public string GenerarReporte(List<Cliente> clientes)
        {
            IReporte reporte = CrearReporte();
            return reporte.Generar(clientes);
        }
    }

    /// <summary>CREATOR concreto: fabrica reportes de texto.</summary>
    public class ReporteTextoCreator : ReporteCreator
    {
        public override IReporte CrearReporte() => new ReporteTexto();
    }

    /// <summary>CREATOR concreto: fabrica reportes CSV.</summary>
    public class ReporteCsvCreator : ReporteCreator
    {
        public override IReporte CrearReporte() => new ReporteCsv();
    }
}
