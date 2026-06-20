using Microsoft.EntityFrameworkCore;
using TransGGP.Domain.Models;

namespace TransGGP.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Operador> Operadores { get; set; }
        public DbSet<Unidad> Unidades { get; set; }
        public DbSet<Semirremolque> Semirremolques { get; set; }
        public DbSet<Dolly> Dollys { get; set; }
        public DbSet<Configuracion> Configuraciones { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
    }
}
