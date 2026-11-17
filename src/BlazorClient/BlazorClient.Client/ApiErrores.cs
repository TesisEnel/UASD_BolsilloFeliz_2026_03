using System.Text.Json;

namespace BlazorClient.Client;

// Convierte la respuesta de error de la API en un texto legible para mostrar en pantalla.
public static class ApiErrores
{
    public static async Task<string> LeerAsync(HttpResponseMessage respuesta)
    {
        var cuerpo = "";
        try
        {
            cuerpo = (await respuesta.Content.ReadAsStringAsync()).Trim();
        }
        catch
        {
            // sin cuerpo legible
        }

        if (cuerpo.StartsWith('{'))
        {
            try
            {
                using var doc = JsonDocument.Parse(cuerpo);
                var raiz = doc.RootElement;

                // Errores de validación de ASP.NET: { "errors": { "Campo": ["mensaje"] } }
                if (raiz.TryGetProperty("errors", out var errores) && errores.ValueKind == JsonValueKind.Object)
                {
                    var mensajes = new List<string>();
                    foreach (var campo in errores.EnumerateObject())
                    {
                        foreach (var m in campo.Value.EnumerateArray())
                        {
                            var texto = m.GetString();
                            if (!string.IsNullOrWhiteSpace(texto))
                                mensajes.Add(texto);
                        }
                    }
                    if (mensajes.Count > 0)
                        return string.Join(" ", mensajes);
                }

                foreach (var nombre in new[] { "mensaje", "detail", "title" })
                {
                    if (raiz.TryGetProperty(nombre, out var valor) &&
                        valor.ValueKind == JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(valor.GetString()))
                    {
                        return valor.GetString()!;
                    }
                }
            }
            catch (JsonException)
            {
                // no era JSON válido
            }
        }
        else if (!string.IsNullOrWhiteSpace(cuerpo))
        {
            // La API devuelve textos simples, por ejemplo: "No existe moneda con id 5."
            return cuerpo.Trim('"');
        }

        return $"Error {(int)respuesta.StatusCode}: {respuesta.ReasonPhrase}";
    }
}
