using Microsoft.EntityFrameworkCore;
using WideWorldImportersBI.Entities.DataWarehouse;

namespace WideWorldImportersBI.Data.DataWarehouse;

public class DwDbContext : DbContext
{
    public DwDbContext(DbContextOptions<DwDbContext> options) : base(options)
    {
    }

    // Fact Tables
    public DbSet<FactVentes> FactVentes { get; set; } = null!;

    // Dimension Tables
    public DbSet<DimClient> DimClients { get; set; } = null!;
    public DbSet<DimProduit> DimProduits { get; set; } = null!;
    public DbSet<DimEmploye> DimEmployes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FactVentes>(entity =>
        {
            entity.HasKey(e => e.VenteSK);
            entity.Property(e => e.PrixUnitaire).HasPrecision(18, 2);
            entity.Property(e => e.MontantHT).HasPrecision(18, 2);
            entity.Property(e => e.Taxe).HasPrecision(18, 2);
            entity.Property(e => e.Profit).HasPrecision(18, 2);

            entity.HasOne(f => f.Client)
                  .WithMany(c => c.Sales)
                  .HasForeignKey(f => f.ClientSK)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(f => f.Produit)
                  .WithMany(p => p.Sales)
                  .HasForeignKey(f => f.ProduitSK)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(f => f.Employe)
                  .WithMany(e => e.Sales)
                  .HasForeignKey(f => f.EmployeSK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DimClient>(entity =>
        {
            entity.HasKey(e => e.ClientSK);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
        });

        modelBuilder.Entity<DimProduit>(entity =>
        {
            entity.HasKey(e => e.ProduitSK);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TaxRate).HasPrecision(18, 3);
        });

        modelBuilder.Entity<DimEmploye>(entity =>
        {
            entity.HasKey(e => e.EmployeSK);
        });
    }
}
