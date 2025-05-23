using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.RecycleService.Data;

/// <summary>
/// 回收站数据库上下文
/// </summary>
public class RecycleDbContext : DbContext
{
    public RecycleDbContext(DbContextOptions<RecycleDbContext> options) : base(options)
    {
    }

    public DbSet<RecycleBin> RecycleBins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RecycleBin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.EntityData).IsRequired();
            entity.Property(e => e.DeleteReason).HasMaxLength(500);
            entity.Property(e => e.DeleteTime).IsRequired();
        });
    }
} 