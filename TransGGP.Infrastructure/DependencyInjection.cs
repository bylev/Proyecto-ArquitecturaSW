using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)));

        return services;
    }
}
