using System.ComponentModel.DataAnnotations;

namespace TransGGP.Domain.Models
{
    public class Dolly
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [StringLength(20, ErrorMessage = "La clave no puede tener más de 20 caracteres.")]
        public string Clave { get; set; } = string.Empty;

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(15, ErrorMessage = "La placa no puede tener más de 15 caracteres.")]
        public string Placa { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
