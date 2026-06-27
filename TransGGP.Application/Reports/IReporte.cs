using System.Text;
using TransGGP.Domain.Models;

namespace TransGGP.Application.Reports
{
    /// <summary>
    /// PRODUCTO abstracto del patrón Factory Method.
    /// Define qué sabe hacer cualquier reporte: generarse a partir de clientes.
    /// </summary>
    public interface IReporte
    {
        string Generar(List<Cliente> clientes);
    }

    /// <summary>PRODUCTO concreto: reporte en texto plano.</summary>
    public class ReporteTexto : IReporte
    {
        public string Generar(List<Cliente> clientes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== REPORTE DE CLIENTES (TEXTO) ===");
            sb.AppendLine($"Total: {clientes.Count} clientes");
            sb.AppendLine();
            foreach (var c in clientes)
                sb.AppendLine($"- [{c.Id}] {c.Nombre} (alta: {c.FechaCreacion:dd/MM/yyyy})");
            return sb.ToString();
        }
    }

    /// <summary>PRODUCTO concreto: reporte en formato CSV.</summary>
    public class ReporteCsv : IReporte
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
