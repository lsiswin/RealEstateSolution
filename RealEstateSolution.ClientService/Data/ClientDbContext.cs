using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Data;

/// <summary>
/// 客户数据库上下文
/// </summary>
public class ClientDbContext : DbContext
{
    public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientRequirements> ClientRequirements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.UpdateTime).IsRequired();

            entity.HasOne(e => e.Requirements)
                .WithOne(r => r.Client)
                .HasForeignKey<ClientRequirements>(r => r.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClientRequirements>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.OtherRequirements).HasMaxLength(500);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.UpdateTime).IsRequired();
        });
    }
} 