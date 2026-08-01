using System.ComponentModel.DataAnnotations;

namespace TransGGP.Domain.Models
{
    public class Operador
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de operador es obligatorio.")]
        [StringLength(20, ErrorMessage = "El número de operador no puede tener más de 20 caracteres.")]
        public string NumeroOperador { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public List<Servicio> Servicios { get; set; } = new();
    }
}
