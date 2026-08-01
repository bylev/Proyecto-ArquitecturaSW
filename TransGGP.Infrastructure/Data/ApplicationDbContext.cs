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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Los servicios son historial permanente: al borrar un cliente,
            // operador o unidad NO se borran sus servicios. La base de datos
            // RESTRINGE el borrado del padre si tiene servicios asociados.
            modelBuilder.Entity<Servicio>()
                .HasOne(s => s.Cliente)
                .WithMany(c => c.Servicios)
                .HasForeignKey(s => s.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Servicio>()
                .HasOne(s => s.Operador)
                .WithMany(o => o.Servicios)
                .HasForeignKey(s => s.OperadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Servicio>()
                .HasOne(s => s.Unidad)
                .WithMany(u => u.Servicios)
                .HasForeignKey(s => s.UnidadId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
