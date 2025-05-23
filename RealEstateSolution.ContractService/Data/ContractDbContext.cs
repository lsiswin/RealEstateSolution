using Microsoft.EntityFrameworkCore;using RealEstateSolution.Database.Models;

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
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Terms).IsRequired();
            entity.Property(e => e.Remark).HasMaxLength(500);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.UpdateTime).IsRequired();

            entity.HasOne<Property>()
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Client>()
                  .WithMany()
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
} 