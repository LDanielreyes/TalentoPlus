using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Entities;

public class Worker : Person
{
    public string Position { get; set; } = string.Empty;
    public int Wage { get; set; }
    public WorkerStatus Status { get; set; }
    public EducationalLevel EducationalLevel { get; set; }
    public Department Department { get; set; }
    public string ProfessionalProfile { get; set; } = string.Empty;
    public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
    
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
