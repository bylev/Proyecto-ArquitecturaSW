using TransGGP.Infrastructure;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Repositories;
using TransGGP.Infrastructure.Decorators;
using TransGGP.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Conexión a MySQL
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddInfrastructure(connectionString);

// Repositorio real (concreto)
builder.Services.AddScoped<ClienteRepository>();

// PATRÓN DECORATOR: cuando alguien pida IClienteRepository, se le entrega
// el decorador de logging, que envuelve al ClienteRepository real.N
builder.Services.AddScoped<IClienteRepository>(provider =>
    new ClienteRepositoryLoggingDecorator(
        provider.GetRequiredService<ClienteRepository>(),
        provider.GetRequiredService<ILogger<ClienteRepositoryLoggingDecorator>>()));

builder.Services.AddScoped<ClienteService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
