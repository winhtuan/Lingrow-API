using Microsoft.EntityFrameworkCore;
using Plantpedia.Models;

namespace Plantpedia.DataAccessLayer.Data;

public class AppDbContext(DbContextOptions<AppDbContext> o) : DbContext(o)
{
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserLoginData> UserLoginDatas => Set<UserLoginData>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // UserAccount
        mb.Entity<UserAccount>(e =>
        {
            e.ToTable("user_account").HasKey(x => x.UserId);
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
            e.HasQueryFilter(x => x.DeletedAt == null); // soft delete
        });

        // UserLoginData (1–1, required) + matching filter
        mb.Entity<UserLoginData>(e =>
        {
            e.ToTable("user_login_data").HasKey(x => x.UserId);

            e.HasOne(x => x.User)
                .WithOne(x => x.LoginData)
                .HasForeignKey<UserLoginData>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict); // tránh xóa dây chuyền khi soft delete

            e.Property(x => x.Role).HasConversion<string>();
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

            // chỉ hiện khi User chưa bị soft-delete
            e.HasQueryFilter(x => x.User != null && x.User.DeletedAt == null);
        });

        // UserActivity (n–1, required) + matching filter
        mb.Entity<UserActivity>(e =>
        {
            e.ToTable("user_activity").HasKey(x => x.ActivityId);

            e.HasOne(x => x.User)
                .WithMany() // KHÔNG có UserAccount.Activities => dùng .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
            e.HasIndex(x => new { x.UserId, x.CreatedAt });
            e.HasIndex(x => x.Type);

            // chỉ hiện khi User chưa bị soft-delete
            e.HasQueryFilter(x => x.User != null && x.User.DeletedAt == null);
        });

        // Seed data
        SeedData(mb);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        Seed.SeedAccount.Apply(modelBuilder);
    }
}
