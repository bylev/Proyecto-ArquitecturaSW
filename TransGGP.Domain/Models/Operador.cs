namespace TransGGP.Domain.Models
{
    public class Operador
    {
        public int Id { get; set; }
        public string NumeroOperador { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Un operador puede tener muchos servicios (relación uno-a-muchos)
        public List<Servicio> Servicios { get; set; } = new();
    }
}
