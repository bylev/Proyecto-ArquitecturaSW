using System.Text;
using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{

    /// PRODUCTO abstracto del patrón Factory Method.
    /// Define qué sabe hacer cualquier reporte: generarse a partir de clientes.

    public interface IReporteCreator
    {
        string Generar(List<Cliente> clientes);
    }

    // REPORTE TXT
    public class ReporteTexto : IReporteCreator
    {
        public string Generar(List<Cliente> clientes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== REPORTE DE CLIENTES ===");
            sb.AppendLine($"Total: {clientes.Count} clientes");
            sb.AppendLine();
            foreach (var c in clientes)
                sb.AppendLine($"- [{c.Id}] {c.Nombre} (Fecha de creacion: {c.FechaCreacion:dd/MM/yyyy})");
            return sb.ToString();
        }
    }

// Reporte CSV
    public class ReporteCsv : IReporteCreator
    {
        public string Generar(List<Cliente> clientes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,FechaCreacion");
            foreach (var c in clientes)
                sb.AppendLine($"{c.Id},{c.Nombre},{c.FechaCreacion:yyyy-MM-dd}");
            return sb.ToString();
        }
    }
}
