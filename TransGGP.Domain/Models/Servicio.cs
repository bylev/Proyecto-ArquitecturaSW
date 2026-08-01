using System.ComponentModel.DataAnnotations;

namespace TransGGP.Domain.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de embarque es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de embarque no puede tener más de 50 caracteres.")]
        public string NumeroEmbarque { get; set; } = string.Empty;

        public string NumeroRemision { get; set; } = string.Empty;
        public string FolioFactura { get; set; } = string.Empty;

        public DateTime FechaCarga {  get; set; } = DateTime.Now;
        public string HoraCita { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El origen es obligatorio.")]
        [StringLength(100, ErrorMessage = "El origen no puede tener más de 100 caracteres.")]
        public string Origen { get; set; } = string.Empty;

        [Required(ErrorMessage = "El destino es obligatorio.")]
        [StringLength(100, ErrorMessage = "El destino no puede tener más de 100 caracteres.")]
        public string Destino { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de carga es obligatorio.")]
        [StringLength(100, ErrorMessage = "El tipo de carga no puede tener más de 100 caracteres.")]
        public string TipoCarga { get; set; } = string.Empty;

        public string Observaciones { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estatus es obligatorio.")]
        [StringLength(50, ErrorMessage = "El estatus no puede tener más de 50 caracteres.")]
        public string Estatus { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente.")]
        public int ClienteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un operador.")]
        public int OperadorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una unidad.")]
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
