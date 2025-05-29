using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.MatchingService.Data;

/// <summary>
/// 匹配数据库上下文
/// </summary>
public class MatchingDbContext : DbContext
{
    public MatchingDbContext(DbContextOptions<MatchingDbContext> options) : base(options)
    {
    }

    public DbSet<Matching> Matchings { get; set; }
    //public DbSet<Client> Clients { get; set; }
    //public DbSet<Property> Properties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Matching>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.UpdateTime).IsRequired();

            entity.HasOne(e => e.Client)
                  .WithMany()
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Property)
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}