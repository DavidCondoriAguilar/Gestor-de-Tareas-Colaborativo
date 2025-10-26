using TaskManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// 1. Obtener la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext para PostgreSQL
var isDev = builder.Environment.IsDevelopment();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    if (isDev)
    {
        // Solo en desarrollo: logs detallados y datos sensibles para depuración
        options.EnableSensitiveDataLogging()
               .EnableDetailedErrors();
    }
});

// Add services to the container.
builder.Services.AddRazorPages();

// Logging de requests/responses HTTP (ideal para depurar con Postman)
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders
        | HttpLoggingFields.ResponsePropertiesAndHeaders
        | HttpLoggingFields.Duration;
    // Opcional: headers adicionales a registrar
    o.RequestHeaders.Add("Authorization");
});

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Aplica migraciones pendientes y asegura el esquema actualizado
    context.Database.Migrate();
    
    // Seed users if they don't exist
    if (!context.Usuarios.Any())
    {
        context.Usuarios.AddRange(
            new TaskManager.Core.Usuario { Nombre = "Juan Pérez" },
            new TaskManager.Core.Usuario { Nombre = "María García" },
            new TaskManager.Core.Usuario { Nombre = "Carlos López" },
            new TaskManager.Core.Usuario { Nombre = "Ana Martínez" }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
