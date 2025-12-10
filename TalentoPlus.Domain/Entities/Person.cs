using Microsoft.AspNetCore.Identity;

namespace TalentoPlus.Domain.Entities;

public class Person : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public string GetContactInfo()
    {
        return $"{FullName} - {Email} - {PhoneNumber}";
    }
}
