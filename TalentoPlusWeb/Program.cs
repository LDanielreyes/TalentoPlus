using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Domain.Entities;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using TalentoPlus.Application.Common.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Application.Services.Impl;
using TalentoPlusWeb.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    // Default scheme for API is JWT, but we need to support Identity Cookies for Web
    // We'll set default to IdentityConstants.ApplicationScheme for the Web App
    // and specify JwtBearerDefaults.AuthenticationScheme for API Controllers
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Swagger to support JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TalentoPlus API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ... existing code ...
builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute());
});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute());
});

// Add DbContext
builder.Services.AddDbContext<TalentoPlus.Infrastructure.Data.ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<TalentoPlus.Domain.Entities.Person, Microsoft.AspNetCore.Identity.IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<TalentoPlus.Infrastructure.Data.ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Default User settings.
    options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+áéíóúÁÉÍÓÚñÑ";
    options.User.RequireUniqueEmail = false;
});

// Register Application Services
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<TalentoPlus.Infrastructure.Data.ApplicationDbContext>());
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddTransient<IEmailService, TalentoPlus.Infrastructure.Services.SmtpEmailService>();
builder.Services.AddScoped<IAIService, TalentoPlus.Infrastructure.Services.GeminiService>();
builder.Services.AddHttpClient();

// Register IEmailSender for Identity email confirmation
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, TalentoPlusWeb.Services.IdentityEmailSender>();

// Override Antiforgery with Dummy implementation to bypass cryptographic errors
builder.Services.AddSingleton<Microsoft.AspNetCore.Antiforgery.IAntiforgery, TalentoPlusWeb.Services.DummyAntiforgery>();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<TalentoPlus.Domain.Entities.Person>>();
        var roleManager = services.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        await TalentoPlus.Infrastructure.Data.DbInitializer.InitializeAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();