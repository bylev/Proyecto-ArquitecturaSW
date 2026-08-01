namespace TransGGP.Application.Dashboards
{
    public class ResumenMes
    {
        public string Mes { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Entregados { get; set; }
        public int EnProceso { get; set; }
        public int Agendados { get; set; }
        public int Cancelados { get; set; }
        public int PorcentajeEntregados { get; set; }
    }

    public class ServicioReporteFila
    {
        public string Embarque { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Operador { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public string Estatus { get; set; } = string.Empty;
    }

    public class ComparativoAnio
    {
        public int Anio { get; set; }
        public int Total { get; set; }
        public int Entregados { get; set; }
        public int Cancelados { get; set; }
        public int PorcentajeEntregados { get; set; }
    }

    public class DashboardResumen
    {
        public int Anio { get; set; }
        public List<int> AniosDisponibles { get; set; } = new List<int>();

        public int Total { get; set; }
        public int Entregados { get; set; }
        public int EnProceso { get; set; }
        public int Agendados { get; set; }
        public int Cancelados { get; set; }
        public int PorcentajeEntregados { get; set; }

        public List<ResumenMes> Meses { get; set; } = new List<ResumenMes>();

        public int TotalHistorico { get; set; }
        public int TotalEntregadosHistorico { get; set; }
        public int TotalCanceladosHistorico { get; set; }
        public int ClientesActivos { get; set; }
        public string MesMasActivo { get; set; } = "Sin datos";
        public string OperadorMasActivo { get; set; } = "Sin datos";

        public List<ComparativoAnio> Comparativo { get; set; } = new List<ComparativoAnio>();
    }
}
