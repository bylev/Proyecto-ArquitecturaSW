namespace TransGGP.Domain.Models
{
    public class Unidad
    {
        public int Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Una unidad puede tener muchos servicios (relación uno-a-muchos)
        public List<Servicio> Servicios { get; set; } = new();
    }
}
