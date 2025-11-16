using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.DataAccessLayer.Data;

public class AppDbContext(DbContextOptions<AppDbContext> o) : DbContext(o)
{
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // UserAccount
        mb.Entity<UserAccount>(e =>
        {
            e.ToTable("user_account").HasKey(x => x.UserId);
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.Role).HasConversion<string>();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.CognitoSub).IsUnique();
            e.HasQueryFilter(x => x.DeletedAt == null); // soft delete
        });

        // UserActivity (n–1)
        mb.Entity<UserActivity>(e =>
        {
            e.ToTable("user_activity").HasKey(x => x.ActivityId);

            e.HasOne(x => x.User)
                .WithMany() // hoặc .WithMany(u => u.Activities) nếu bạn thêm collection
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
            e.HasIndex(x => new { x.UserId, x.CreatedAt });

            // MATCH filter với UserAccount
            e.HasQueryFilter(a => a.User.DeletedAt == null);
        });

        SeedData(mb);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        Seed.SeedAccount.Apply(modelBuilder);
    }
}
