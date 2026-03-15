using ElectroHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectroHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Productos> Productos { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Proveedores> Proveedores { get; set; }
        public DbSet<Ventas> Ventas { get; set; }
        public DbSet<DetallesVentas> DetallesVentas { get; set; }
        public DbSet<InventarioMovimientos> InventarioMovimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InventarioMovimientos>().ToTable("InventarioMovimientos");
        }
    }
}