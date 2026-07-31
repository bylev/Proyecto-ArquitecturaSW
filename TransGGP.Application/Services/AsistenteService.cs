using System.Text;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Dashboards;

namespace TransGGP.Application.Services
{
    public class AsistenteService
    {
        private readonly IAsistenteAnalisis _asistente;

        public AsistenteService(IAsistenteAnalisis asistente)
        {
            _asistente = asistente;
        }

        public Task<string> ResponderAsync(DashboardResumen resumen, List<MensajeChat> conversacion)
        {
            var contexto = ConstruirContexto(resumen);
            return _asistente.ResponderAsync(contexto, conversacion);
        }

        private static string ConstruirContexto(DashboardResumen r)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Año analizado: {r.Anio}");
            sb.AppendLine($"Servicios del año: {r.Total} (Entregados: {r.Entregados}, En proceso: {r.EnProceso}, Agendados: {r.Agendados}, Cancelados: {r.Cancelados}, Porcentaje entregados: {r.PorcentajeEntregados}%)");
            sb.AppendLine("Servicios por mes:");
            foreach (var m in r.Meses)
                sb.AppendLine($"- {m.Mes}: {m.Total} servicios (entregados {m.Entregados})");
            sb.AppendLine($"Total histórico de servicios: {r.TotalHistorico}");
            sb.AppendLine($"Clientes activos: {r.ClientesActivos}");
            sb.AppendLine($"Mes más activo: {r.MesMasActivo}");
            sb.AppendLine($"Operador más activo: {r.OperadorMasActivo}");
            sb.AppendLine("Comparativo por año:");
            foreach (var c in r.Comparativo)
                sb.AppendLine($"- {c.Anio}: {c.Total} servicios (entregados {c.Entregados}, cancelados {c.Cancelados}, porcentaje entregados {c.PorcentajeEntregados}%)");
            return sb.ToString();
        }
    }
}
