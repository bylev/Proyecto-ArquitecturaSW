namespace TransGGP.Domain.Models
{
    public class Dolly
    {
        public int Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
