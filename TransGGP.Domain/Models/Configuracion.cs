using System.ComponentModel.DataAnnotations;

namespace TransGGP.Domain.Models
{
    public class Configuracion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}
