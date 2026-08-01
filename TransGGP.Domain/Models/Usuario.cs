namespace TransGGP.Domain.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty; // "Admin" o "User"

        public bool PuedeVerServicios { get; set; }
        public bool PuedeEditarServicios { get; set; }
        public bool PuedeVerClientes { get; set; }
        public bool PuedeEditarClientes { get; set; }
        public bool PuedeVerOperadores { get; set; }
        public bool PuedeEditarOperadores { get; set; }
        public bool PuedeVerUnidades { get; set; }
        public bool PuedeEditarUnidades { get; set; }
        public bool PuedeVerSemirremolques { get; set; }
        public bool PuedeEditarSemirremolques { get; set; }
        public bool PuedeVerDollys { get; set; }
        public bool PuedeEditarDollys { get; set; }
        public bool PuedeVerConfiguraciones { get; set; }
        public bool PuedeEditarConfiguraciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
