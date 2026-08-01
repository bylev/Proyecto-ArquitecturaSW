using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransGGP.Application.Interfaces
{
    public class MensajeChat
    {
        public string Rol { get; set; } = "user";
        public string Contenido { get; set; } = string.Empty;
    }

    public interface IAsistenteAnalisis
    {
        Task<string> ResponderAsync(string contextoNegocio, List<MensajeChat> conversacion);
    }
}
