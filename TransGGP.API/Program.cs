using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Security;
using TransGGP.Application.Services;
using TransGGP.Infrastructure;
using TransGGP.Infrastructure.Data;
using TransGGP.Infrastructure.Repositories;
using TransGGP.Infrastructure.Security;
using TransGGP.Application.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios
builder.Services.AddControllers(options =>
{
    // Permite POST con solo los campos necesarios (campos string omitidos no son obligatorios)
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Inyección de dependencias
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<IOperadorRepository, OperadorRepository>();
builder.Services.AddScoped<OperadorService>();
builder.Services.AddScoped<IUnidadRepository, UnidadRepository>();
builder.Services.AddScoped<UnidadService>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<ServicioService>();

// Seguridad: usuarios y hasheo (para el login que emite el token)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<UsuarioService>();

// Autenticación con token JWT: valida el token que llega en cada petición
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:Key debe existir y tener al menos 32 bytes.");

if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Jwt:Issuer y Jwt:Audience son obligatorios.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Botón "Authorize" en Swagger para pegar el token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega aquí el token (sin escribir 'Bearer')."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

// Detrás del balanceador de AWS y de Cloudflare: respetar el esquema original (X-Forwarded-Proto)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Manejador global de errores: convierte ValidacionException en un 400 con mensaje
app.UseExceptionHandler(manejador =>
{
    manejador.Run(async contexto =>
    {
        var error = contexto.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (error is ValidacionException)
        {
            contexto.Response.StatusCode = StatusCodes.Status400BadRequest;
            await contexto.Response.WriteAsJsonAsync(new { error = error.Message });
        }
        else
        {
            contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await contexto.Response.WriteAsJsonAsync(new { error = "Ocurrió un error en el servidor." });
        }
    });
});

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "docs/swagger/{documentName}.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/docs/swagger/v1.json", "TransGGP API");
        options.RoutePrefix = "docs";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Sembrado: si el API se ejecuta solo y la base está vacía, crea un administrador inicial.
using (var scope = app.Services.CreateScope())
{
    var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
    if (!usuarioService.ExisteAlgunUsuario())
    {
        usuarioService.RegistrarUsuario("Administrador", "admin@transggp.com", "Admin123!", "Admin");
    }
}

app.Run();
