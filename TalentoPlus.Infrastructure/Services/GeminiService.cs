using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TalentoPlus.Application.Services;

namespace TalentoPlus.Infrastructure.Services;

public class GeminiService(IConfiguration configuration, ILogger<GeminiService> logger, IHttpClientFactory httpClientFactory) : IAIService
{
    private readonly string? _apiKey = configuration["GeminiSettings:ApiKey"];

    public async Task<AIQueryResponse> ProcessQueryAsync(string query)
    {
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            logger.LogWarning("Gemini API key not configured");
            return new AIQueryResponse
            {
                QueryType = "count_workers",
                Parameter = ""
            };
        }

        try
        {
            var prompt = $@"Analiza esta pregunta sobre empleados y devuelve SOLO un JSON con este formato exacto:
{{""queryType"": ""tipo"", ""parameter"": ""parametro""}}

Tipos válidos:
- count_workers: contar todos los empleados
- count_by_status: contar por estado (Activo, Inactivo, Vacaciones)
- count_by_department: contar por departamento (Marketing, Operaciones, RecursosHumanos, Logística, Ventas, Contabilidad, Tecnología)
- count_by_position: contar por cargo/puesto
- count_by_education: contar por nivel educativo (Tecnico, Tecnólogo, Profesional, Maestría, Especialización)

Pregunta del usuario: {query}

Responde SOLO con el JSON, sin texto adicional.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var client = httpClientFactory.CreateClient();
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";
            logger.LogInformation("Calling Gemini API v1beta with model: gemini-2.0-flash");
            
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, jsonContent);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                // Para errores de autenticación/permisos (API key inválida/filtrada), usar Warning en lugar de Error
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden || 
                    response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    logger.LogWarning("Gemini API key is invalid or has been revoked. AI features are disabled. Status: {StatusCode}", response.StatusCode);
                }
                else
                {
                    logger.LogError("Gemini API request failed: {StatusCode}. Response: {Response}", response.StatusCode, responseContent);
                }
                return new AIQueryResponse { QueryType = "count_workers", Parameter = "" };
            }

            var geminiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            var text = geminiResponse
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "{}";

            // Extract JSON from markdown code block if present
            text = text.Trim();
            if (text.StartsWith("```json"))
            {
                text = text.Substring(7);
                text = text.Substring(0, text.LastIndexOf("```"));
            }
            else if (text.StartsWith("```"))
            {
                text = text.Substring(3);
                text = text.Substring(0, text.LastIndexOf("```"));
            }

            var aiResponse = JsonSerializer.Deserialize<AIQueryResponse>(text.Trim(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return aiResponse ?? new AIQueryResponse { QueryType = "count_workers", Parameter = "" };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing AI query");
            return new AIQueryResponse { QueryType = "count_workers", Parameter = "" };
        }
    }
}
