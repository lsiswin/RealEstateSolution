using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Data;

/// <summary>
/// 房源数据库上下文
/// </summary>
public class PropertyDbContext : DbContext
{
    public PropertyDbContext(DbContextOptions<PropertyDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Address).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Area).IsRequired();
            entity.Property(e => e.Price).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.OwnerId).IsRequired();
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.UpdateTime).IsRequired();
        });

        modelBuilder.Entity<PropertyImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.PropertyId).IsRequired();
            entity.Property(e => e.CreateTime).IsRequired();
            entity.HasOne(e => e.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 