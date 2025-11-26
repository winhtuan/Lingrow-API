using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.DataAccessLayer.Data;

public class AppDbContext(DbContextOptions<AppDbContext> o) : DbContext(o)
{
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();
    public DbSet<StudentCard> StudentCards => Set<StudentCard>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Tutor> Tutors => Set<Tutor>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ===========================
        // USERACCOUNT (TPH)
        // ===========================
        mb.Entity<UserAccount>(e =>
        {
            e.ToTable("user_account").HasKey(x => x.UserId);

            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.Role).HasConversion<string>();

            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.CognitoSub).IsUnique();

            e.HasQueryFilter(x => x.DeletedAt == null);
        });

        mb.Entity<UserAccount>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<UserAccount>("UserAccount");

        // Student → kế thừa UserAccount
        mb.Entity<Student>().ToTable("user_account");
        mb.Entity<Tutor>().ToTable("user_account");

        // ===========================
        // USER ACTIVITY
        // ===========================
        mb.Entity<UserActivity>(e =>
        {
            e.ToTable("user_activity").HasKey(x => x.ActivityId);

            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

            e.HasIndex(x => new { x.UserId, x.CreatedAt });

            e.HasQueryFilter(a => a.User.DeletedAt == null);
        });

        // STUDENT CARD
        mb.Entity<StudentCard>(e =>
        {
            e.ToTable("student_cards").HasKey(x => x.Id);

            e.Property(x => x.Color).HasMaxLength(50);
            e.Property(x => x.DisplayName).HasMaxLength(200);
            e.Property(x => x.Tags).HasColumnType("text");

            e.HasOne(x => x.Tutor)
                .WithMany(u => u.StudentCardsAsTutor)
                .HasForeignKey(x => x.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(c => c.Tutor != null && c.Tutor.DeletedAt == null);
        });

        // SCHEDULE
        mb.Entity<Schedule>(e =>
        {
            e.ToTable("schedules").HasKey(x => x.Id);

            e.Property(x => x.Title).HasMaxLength(200);
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.Type).HasConversion<string>();

            e.HasOne(s => s.StudentCard)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.StudentCardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(s => s.Tutor)
                .WithMany(u => u.SchedulesAsTutor)
                .HasForeignKey(s => s.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.TutorId, x.StartTime });
            e.HasIndex(x => new { x.StudentCardId, x.StartTime });

            // Soft delete + đồng bộ với filter UserAccount
            e.HasQueryFilter(s => !s.IsDeleted && s.Tutor!.DeletedAt == null);
        });

        // ===========================
        // SEED DATA
        // ===========================
        SeedData(mb);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        Seed.SeedAccount.Apply(modelBuilder);
    }
}
