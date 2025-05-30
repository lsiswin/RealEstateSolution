using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Data;

/// <summary>
/// 合同数据库上下文
/// </summary>
public class ContractDbContext : DbContext
{
    public ContractDbContext(DbContextOptions<ContractDbContext> options) : base(options)
    {
    }

    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<ContractTemplate> ContractTemplates { get; set; } = null!;
    public DbSet<Property> Property { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContractNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PropertyId).IsRequired();
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ClientId1).IsRequired(false);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired(false);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Terms).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Remark).HasMaxLength(500);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.UpdateTime).IsRequired();

            entity.HasOne(e => e.Property)
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Client)
                  .WithMany()
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Client1)
                  .WithMany()
                  .HasForeignKey(e => e.ClientId1)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ContractTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Content).IsRequired().HasColumnType("ntext");
            entity.Property(e => e.Version).IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.FileSize).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // 创建索引
            entity.HasIndex(e => e.Type).HasDatabaseName("IX_ContractTemplates_Type");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_ContractTemplates_IsActive");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_ContractTemplates_CreatedAt");
        });
    }
} 