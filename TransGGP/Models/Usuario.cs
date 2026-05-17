using System.Globalization;

namespace TransGGP.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty; // "Admin" o "User"

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

    }
}
