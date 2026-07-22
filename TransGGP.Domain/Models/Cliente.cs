namespace TransGGP.Domain.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Un cliente puede tener muchos servicios (relación uno-a-muchos)
        public List<Servicio> Servicios { get; set; } = new();
    }
}
