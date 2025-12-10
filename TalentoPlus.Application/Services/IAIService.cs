namespace TalentoPlus.Application.Services;

public interface IAIService
{
    Task<AIQueryResponse> ProcessQueryAsync(string query);
}

public class AIQueryResponse
{
    public string QueryType { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
}
