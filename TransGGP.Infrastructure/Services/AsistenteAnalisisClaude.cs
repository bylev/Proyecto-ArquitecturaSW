using System.Text;
using System.Text.Json;
using TransGGP.Application.Interfaces;

namespace TransGGP.Infrastructure.Services
{
    public class AsistenteAnalisisClaude : IAsistenteAnalisis
    {
        private static readonly HttpClient Http = new HttpClient();

        private readonly string _apiKey;
        private readonly string _modelo;

        public AsistenteAnalisisClaude(string apiKey, string modelo)
        {
            _apiKey = apiKey;
            _modelo = string.IsNullOrWhiteSpace(modelo) ? "claude-haiku-4-5-20251001" : modelo;
        }

        public async Task<string> ResponderAsync(string contextoNegocio, List<MensajeChat> conversacion)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("No hay una API key de Anthropic configurada. Agrégala en la configuración (Anthropic:ApiKey).");

            var instruccion = "Eres el asistente virtual de Transportes GGP, una empresa familiar de transporte de carga. " +
                "Ayudas al dueño a entender cómo va su negocio. Usa EXCLUSIVAMENTE las cifras que aparecen en los DATOS DEL NEGOCIO, " +
                "no inventes ni supongas números; si te preguntan algo que no está en los datos, dilo con sinceridad. " +
                "Responde en español, breve y claro, con un tono cercano y profesional (el dueño no es técnico). " +
                "Cuando tenga sentido, di si la empresa está creciendo o no y da recomendaciones prácticas.\n\n" +
                "DATOS DEL NEGOCIO:\n" + contextoNegocio;

            var mensajes = conversacion
                .Select(m => new
                {
                    role = m.Rol == "assistant" ? "assistant" : "user",
                    content = m.Contenido
                })
                .ToList();

            var cuerpo = new
            {
                model = _modelo,
                max_tokens = 1024,
                system = instruccion,
                messages = mensajes
            };

            var json = JsonSerializer.Serialize(cuerpo);

            using var solicitud = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
            solicitud.Headers.Add("x-api-key", _apiKey);
            solicitud.Headers.Add("anthropic-version", "2023-06-01");
            solicitud.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var respuesta = await Http.SendAsync(solicitud);
            var textoRespuesta = await respuesta.Content.ReadAsStringAsync();

            if (!respuesta.IsSuccessStatusCode)
                throw new InvalidOperationException($"La IA respondió con un error ({(int)respuesta.StatusCode}). Revisa la API key o el modelo configurado.");

            using var documento = JsonDocument.Parse(textoRespuesta);
            var contenido = documento.RootElement.GetProperty("content");

            var resultado = new StringBuilder();
            foreach (var bloque in contenido.EnumerateArray())
            {
                if (bloque.GetProperty("type").GetString() == "text")
                    resultado.Append(bloque.GetProperty("text").GetString());
            }

            return resultado.ToString().Trim();
        }
    }
}
