namespace TalentoPlus.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    
    // Foreign Key
    public string WorkerId { get; set; } = string.Empty;
    public Worker? Worker { get; set; }
}
