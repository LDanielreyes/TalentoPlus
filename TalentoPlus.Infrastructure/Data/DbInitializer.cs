using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace TalentoPlus.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(UserManager<Person> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        await CreateRolesAsync(roleManager);
        
        // Seed Admin User
        await SeedAdminUserAsync(userManager);
        
        // Assign Worker role to all existing workers
        await AssignWorkerRolesAsync(userManager);
    }
    
    private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "Worker" };
        
        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    Console.WriteLine($"Role '{roleName}' created successfully");
                }
            }
        }
    }
    
    private static async Task SeedAdminUserAsync(UserManager<Person> userManager)
    {
        var adminEmail = "admin@talentoplus.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new Admin
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                Address = "Talento Plus HQ",
                EmailConfirmed = true,
                LastLogin = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin user '{adminEmail}' created and assigned to Admin role");
            }
        }
        else
        {
            // Ensure existing admin has Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin role assigned to existing user '{adminEmail}'");
            }
        }
    }
    
    private static async Task AssignWorkerRolesAsync(UserManager<Person> userManager)
    {
        // Get all workers
        var allUsers = await userManager.Users.ToListAsync();
        var workers = allUsers.OfType<Worker>().ToList();
        
        foreach (var worker in workers)
        {
            if (!await userManager.IsInRoleAsync(worker, "Worker"))
            {
                await userManager.AddToRoleAsync(worker, "Worker");
                Console.WriteLine($"Worker role assigned to '{worker.Email}'");
            }
        }
    }
}
