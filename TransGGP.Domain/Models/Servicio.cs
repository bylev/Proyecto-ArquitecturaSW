namespace TransGGP.Domain.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string NumeroEmbarque { get; set; } = string.Empty;
        public string NumeroRemision { get; set; } = string.Empty;
        public string FolioFactura { get; set; } = string.Empty;

        public DateTime FechaCarga {  get; set; } = DateTime.Now;
        public string HoraCita { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; } = DateTime.Now;

        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string TipoCarga { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones

        public int ClienteId { get; set; }
        public int OperadorId { get; set; }
        public int UnidadId { get; set; }
        public int? SemirremolqueId { get; set; }
        public int? DollyId { get; set; }
        public int? ConfiguracionId { get; set; }   // opcional

        // Propiedades de navegación (relaciones entre tablas)
        public Cliente? Cliente { get; set; }                 // obligatoria
        public Operador? Operador { get; set; }               // obligatoria
        public Unidad? Unidad { get; set; }                   // obligatoria
        public Semirremolque? Semirremolque { get; set; }     // opcional
        public Dolly? Dolly { get; set; }                     // opcional
        public Configuracion? Configuracion { get; set; }     // opcional
    }
}
