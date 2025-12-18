using InventoryWebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseInventory> WarehouseInventories => Set<WarehouseInventory>();
    public DbSet<LookupGroup> LookupGroups => Set<LookupGroup>();
    public DbSet<LookupItem> LookupItems => Set<LookupItem>();

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Enforce server-side audit values (ignore any request payload values)
                    entry.Property(x => x.CreatedAt).CurrentValue = utcNow;
                    entry.Property(x => x.UpdatedAt).CurrentValue = null;
                    break;

                case EntityState.Modified:
                    // Never allow CreatedAt to be changed once persisted
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.UpdatedAt).CurrentValue = utcNow;
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure audit properties for all entities that inherit from AuditableEntity
        ConfigureAuditableEntity<Product>(modelBuilder);
        ConfigureAuditableEntity<Warehouse>(modelBuilder);
        ConfigureAuditableEntity<WarehouseInventory>(modelBuilder);
        ConfigureAuditableEntity<LookupGroup>(modelBuilder);
        ConfigureAuditableEntity<LookupItem>(modelBuilder);

        ConfigureProduct(modelBuilder);
        ConfigureWarehouse(modelBuilder);
        ConfigureWarehouseInventory(modelBuilder);
        ConfigureLookupGroup(modelBuilder);
        ConfigureLookupItem(modelBuilder);
    }

    private static void ConfigureAuditableEntity<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        var entity = modelBuilder.Entity<T>();

        entity.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        entity.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        entity.Property(x => x.UpdatedAt);

        entity.Property(x => x.UpdatedBy)
            .HasMaxLength(100);
    }

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Product>();

        entity.ToTable("Products");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        entity.Property(x => x.SkuId)
            .HasMaxLength(20);

        entity.HasIndex(x => x.SkuId)
            .IsUnique()
            .HasFilter("[SkuId] IS NOT NULL"); // Allow multiple nulls in unique index

        entity.Property(x => x.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(x => x.Category)
            .HasMaxLength(200);

        entity.Property(x => x.Uom)
            .HasMaxLength(20);

        entity.Property(x => x.IsActive)
            .HasDefaultValue(true);

        entity.Property(x => x.ProductMetadata)
            .HasColumnType("nvarchar(max)");

        entity.Property(x => x.IsBundled)
            .HasDefaultValue(false);

        entity.Property(x => x.Rate)
            .HasColumnType("decimal(15,2)")
            .HasDefaultValue(0m);

        entity.Property(x => x.Tax)
            .HasColumnType("decimal(15,2)")
            .HasDefaultValue(0m);
    }

    private static void ConfigureWarehouse(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Warehouse>();

        entity.ToTable("Warehouses");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        entity.Property(x => x.Name)
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(x => x.Address)
            .HasMaxLength(200);
    }

    private static void ConfigureWarehouseInventory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<WarehouseInventory>();

        entity.ToTable("WarehouseInventories");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        entity.Property(x => x.ProductId)
            .IsRequired();

        entity.Property(x => x.WarehouseId)
            .IsRequired();

        entity.Property(x => x.TotalQty)
            .HasDefaultValue(0);

        entity.Property(x => x.AvailableQty)
            .HasDefaultValue(0);

        entity.Property(x => x.ReservedQty)
            .HasDefaultValue(0);

        entity.Property(x => x.BlockedQty)
            .HasDefaultValue(0);

        entity.HasOne(x => x.Product)
            .WithMany(p => p.WarehouseInventories)
            .HasForeignKey(x => x.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(x => x.Warehouse)
            .WithMany(w => w.WarehouseInventories)
            .HasForeignKey(x => x.WarehouseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Composite unique index to prevent duplicate product-warehouse combinations
        entity.HasIndex(x => new { x.ProductId, x.WarehouseId })
            .IsUnique();
    }

    private static void ConfigureLookupGroup(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<LookupGroup>();

        entity.ToTable("LookupGroups");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        entity.Property(x => x.Name)
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(x => x.Description)
            .HasMaxLength(200);
    }

    private static void ConfigureLookupItem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<LookupItem>();

        entity.ToTable("LookupItems");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        entity.Property(x => x.Name)
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(x => x.Description)
            .HasMaxLength(200);

        entity.Property(x => x.GroupId)
            .IsRequired();

        entity.HasOne(x => x.Group)
            .WithMany(g => g.Items)
            .HasForeignKey(x => x.GroupId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
