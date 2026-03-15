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

            // Configurar relaciones con NO ACTION
            modelBuilder.Entity<DetallesVentas>()
                .HasOne(d => d.Categoria)
                .WithMany()
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DetallesVentas>()
                .HasOne(d => d.Productos)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DetallesVentas>()
                .HasOne(d => d.Ventas)
                .WithMany(v => v.DetallesVentas)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventarioMovimientos>().ToTable("InventarioMovimientos");
        }
    }
}