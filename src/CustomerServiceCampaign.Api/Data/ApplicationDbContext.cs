using CustomerServiceCampaign.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaign.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Campaign> Campaigns => Set<Campaign>();

    public DbSet<Agent> Agents => Set<Agent>();

    public DbSet<Reward> Rewards => Set<Reward>();

    public DbSet<Purchase> Purchases => Set<Purchase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Agent>(entity =>
        {
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.Property(x => x.CustomerId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => new { x.CampaignId, x.CustomerId })
                .IsUnique();

            entity.HasOne(x => x.Campaign)
                .WithMany()
                .HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Agent)
                .WithMany()
                .HasForeignKey(x => x.AgentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.Property(x => x.CustomerId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.PurchaseReference)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.PurchaseReference)
                .IsUnique();
        });
    }
}