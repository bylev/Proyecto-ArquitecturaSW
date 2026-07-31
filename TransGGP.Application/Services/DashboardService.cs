using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Dashboards;

namespace TransGGP.Application.Services
{
    public class DashboardService
    {
        private readonly IServicioRepository _servicioRepository;
        private readonly IOperadorRepository _operadorRepository;
        private readonly IClienteRepository _clienteRepository;

        private static readonly string[] NombresMeses =
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };

        private const string Entregado = "Entregado";
        private const string EnProceso = "En proceso";
        private const string Agendado = "Agendado";
        private const string Cancelado = "Cancelado";

        public DashboardService(IServicioRepository servicioRepository, IOperadorRepository operadorRepository, IClienteRepository clienteRepository)
        {
            _servicioRepository = servicioRepository;
            _operadorRepository = operadorRepository;
            _clienteRepository = clienteRepository;
        }

        public List<ServicioReporteFila> ObtenerServiciosDelAnio(int anio)
        {
            var clientes = _clienteRepository.ObtenerTodos().ToDictionary(c => c.Id, c => c.Nombre);
            var operadores = _operadorRepository.ObtenerTodos().ToDictionary(o => o.Id, o => o.Nombre);

            return _servicioRepository.ObtenerTodos()
                .Where(s => s.FechaCarga.Year == anio)
                .OrderBy(s => s.FechaCarga)
                .Select(s => new ServicioReporteFila
                {
                    Embarque = s.NumeroEmbarque,
                    Cliente = clientes.ContainsKey(s.ClienteId) ? clientes[s.ClienteId] : "-",
                    Operador = operadores.ContainsKey(s.OperadorId) ? operadores[s.OperadorId] : "-",
                    Origen = s.Origen,
                    Destino = s.Destino,
                    FechaCarga = s.FechaCarga,
                    Estatus = s.Estatus
                })
                .ToList();
        }

        public DashboardResumen Generar(int? anio)
        {
            var servicios = _servicioRepository.ObtenerTodos();

            var resumen = new DashboardResumen();

            resumen.AniosDisponibles = servicios
                .Select(s => s.FechaCarga.Year)
                .Distinct()
                .OrderByDescending(a => a)
                .ToList();

            if (!resumen.AniosDisponibles.Contains(DateTime.Now.Year))
                resumen.AniosDisponibles.Insert(0, DateTime.Now.Year);

            resumen.Anio = anio ?? resumen.AniosDisponibles.First();

            var serviciosDelAnio = servicios.Where(s => s.FechaCarga.Year == resumen.Anio).ToList();

            resumen.Total = serviciosDelAnio.Count;
            resumen.Entregados = Contar(serviciosDelAnio, Entregado);
            resumen.EnProceso = Contar(serviciosDelAnio, EnProceso);
            resumen.Agendados = Contar(serviciosDelAnio, Agendado);
            resumen.Cancelados = Contar(serviciosDelAnio, Cancelado);
            resumen.PorcentajeEntregados = Porcentaje(resumen.Entregados, resumen.Total);

            for (int mes = 1; mes <= 12; mes++)
            {
                var delMes = serviciosDelAnio.Where(s => s.FechaCarga.Month == mes).ToList();
                resumen.Meses.Add(new ResumenMes
                {
                    Mes = NombresMeses[mes - 1],
                    Total = delMes.Count,
                    Entregados = Contar(delMes, Entregado),
                    EnProceso = Contar(delMes, EnProceso),
                    Agendados = Contar(delMes, Agendado),
                    Cancelados = Contar(delMes, Cancelado),
                    PorcentajeEntregados = Porcentaje(Contar(delMes, Entregado), delMes.Count)
                });
            }

            resumen.TotalHistorico = servicios.Count;
            resumen.TotalEntregadosHistorico = Contar(servicios, Entregado);
            resumen.TotalCanceladosHistorico = Contar(servicios, Cancelado);
            resumen.ClientesActivos = servicios.Select(s => s.ClienteId).Distinct().Count();
            resumen.MesMasActivo = MesMasActivo(servicios);
            resumen.OperadorMasActivo = OperadorMasActivo(servicios);

            resumen.Comparativo = servicios
                .Select(s => s.FechaCarga.Year)
                .Distinct()
                .OrderBy(a => a)
                .Select(a =>
                {
                    var delAnio = servicios.Where(s => s.FechaCarga.Year == a).ToList();
                    return new ComparativoAnio
                    {
                        Anio = a,
                        Total = delAnio.Count,
                        Entregados = Contar(delAnio, Entregado),
                        Cancelados = Contar(delAnio, Cancelado),
                        PorcentajeEntregados = Porcentaje(Contar(delAnio, Entregado), delAnio.Count)
                    };
                })
                .ToList();

            return resumen;
        }

        private static int Contar(List<Servicio> servicios, string estatus)
        {
            return servicios.Count(s => string.Equals((s.Estatus ?? string.Empty).Trim(), estatus, StringComparison.OrdinalIgnoreCase));
        }

        private static int Porcentaje(int parte, int total)
        {
            if (total == 0)
                return 0;

            return (int)Math.Round((double)parte * 100 / total);
        }

        private string MesMasActivo(List<Servicio> servicios)
        {
            if (servicios.Count == 0)
                return "Sin datos";

            var mes = servicios
                .GroupBy(s => s.FechaCarga.Month)
                .OrderByDescending(g => g.Count())
                .First().Key;

            return NombresMeses[mes - 1];
        }

        private string OperadorMasActivo(List<Servicio> servicios)
        {
            if (servicios.Count == 0)
                return "Sin datos";

            var operadorId = servicios
                .GroupBy(s => s.OperadorId)
                .OrderByDescending(g => g.Count())
                .First().Key;

            var operador = _operadorRepository.ObtenerPorId(operadorId);
            return operador?.Nombre ?? "Sin datos";
        }
    }
}
