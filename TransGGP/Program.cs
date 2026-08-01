using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Infrastructure;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Security;
using TransGGP.Infrastructure.Repositories;
using TransGGP.Infrastructure.Decorators;
using TransGGP.Infrastructure.Security;
using TransGGP.Infrastructure.Services;
using TransGGP.Application.Services;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Conexión a MySQL
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddInfrastructure(connectionString);

// Repositorio real (concreto)
builder.Services.AddScoped<ClienteRepository>();

// PATRÓN DECORATOR: cuando alguien pida IClienteRepository, se le entrega
// el decorador de logging, que envuelve al ClienteRepository real.
builder.Services.AddScoped<IClienteRepository>(provider =>
    new ClienteRepositoryLoggingDecorator(
        provider.GetRequiredService<ClienteRepository>(),
        provider.GetRequiredService<ILogger<ClienteRepositoryLoggingDecorator>>()));

builder.Services.AddScoped<ClienteService>();

// Repositorios y servicios de Operador, Unidad y Servicio
builder.Services.AddScoped<IOperadorRepository, OperadorRepository>();
builder.Services.AddScoped<OperadorService>();
builder.Services.AddScoped<IUnidadRepository, UnidadRepository>();
builder.Services.AddScoped<UnidadService>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<ServicioService>();
builder.Services.AddScoped<ISemirremolqueRepository, SemirremolqueRepository>();
builder.Services.AddScoped<SemirremolqueService>();
builder.Services.AddScoped<IDollyRepository, DollyRepository>();
builder.Services.AddScoped<DollyService>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<ConfiguracionService>();

// Seguridad: hasheo de contraseñas y gestión de usuarios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IAsistenteAnalisis>(_ => new AsistenteAnalisisClaude(
    builder.Configuration["Anthropic:ApiKey"] ?? "",
    builder.Configuration["Anthropic:Model"] ?? "claude-haiku-4-5-20251001"));
builder.Services.AddScoped<AsistenteService>();
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

// Autenticación por cookie: guarda la sesión del usuario tras iniciar sesión
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    });

// Por defecto, toda la app exige haber iniciado sesión (salvo [AllowAnonymous])
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    foreach (var permiso in Permisos.Todos())
    {
        var permisoActual = permiso;
        options.AddPolicy(permisoActual, policy =>
            policy.RequireAssertion(context => context.User.TienePermiso(permisoActual)));
    }
});

builder.Services.AddControllersWithViews(options =>
{
    // Los campos string no incluidos en el formulario no se marcan como obligatorios
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    // Protección CSRF: valida el token antiforgery en todos los formularios POST
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// La app corre detrás del balanceador de AWS y de Cloudflare. Ellos terminan el HTTPS
// y nos avisan del esquema original con la cabecera X-Forwarded-Proto. Sin esto la app
// cree que todo llega por HTTP y entra en un bucle infinito de redirecciones.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Las IP del balanceador cambian solas, por eso no se listan.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Las llaves que cifran la cookie de sesión se guardan en disco. Si no se conserva
// la carpeta entre despliegues, a todos se les cierra la sesión al actualizar la app.
var rutaLlaves = builder.Configuration["DataProtection:KeysPath"];
if (!string.IsNullOrWhiteSpace(rutaLlaves))
{
    // Si la carpeta no se puede usar (permisos del servidor), la app NO debe caerse:
    // se sigue sin persistir las llaves, que solo implica volver a iniciar sesión.
    try
    {
        Directory.CreateDirectory(rutaLlaves);
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(rutaLlaves))
            .SetApplicationName("TransGGP");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Aviso] No se pudieron guardar las llaves en '{rutaLlaves}': {ex.Message}");
    }
}

var app = builder.Build();

// Debe ir de primero, antes de cualquier middleware que mire el esquema o la IP.
app.UseForwardedHeaders();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Sembrado: si no hay ningún usuario, crea un administrador inicial.
// En producción el correo y la contraseña se toman de la configuración
// (variables de entorno), nunca del código.
using (var scope = app.Services.CreateScope())
{
    var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
    if (!usuarioService.ExisteAlgunUsuario())
    {
        var correoAdmin = app.Configuration["AdminInicial:Email"] ?? "admin@transggp.com";
        var passwordAdmin = app.Configuration["AdminInicial:Password"];

        if (string.IsNullOrWhiteSpace(passwordAdmin))
        {
            if (app.Environment.IsDevelopment())
            {
                passwordAdmin = "Admin123!";
            }
            else
            {
                throw new InvalidOperationException(
                    "No hay usuarios y falta configurar AdminInicial:Password para crear el administrador.");
            }
        }

        usuarioService.RegistrarUsuario("Administrador", correoAdmin, passwordAdmin, "Admin");
    }
}

app.Run();
