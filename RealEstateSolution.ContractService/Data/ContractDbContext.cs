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
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContractNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PropertyId).IsRequired();
            entity.Property(e => e.PartyAId).IsRequired();
            entity.Property(e => e.PartyBId).IsRequired();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(450);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(e => e.Property)
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PartyA)
                  .WithMany()
                  .HasForeignKey(e => e.PartyAId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PartyB)
                  .WithMany()
                  .HasForeignKey(e => e.PartyBId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
} 