using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Domain.Entities;

using TalentoPlus.Application.Common.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Application.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<TalentoPlus.Infrastructure.Data.ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<TalentoPlus.Domain.Entities.Person, Microsoft.AspNetCore.Identity.IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<TalentoPlus.Infrastructure.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register Application Services
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<TalentoPlus.Infrastructure.Data.ApplicationDbContext>());
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();