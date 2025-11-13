// Plantpedia.DataAccessLayer/Data/Seed/SeedAccount.cs
using Microsoft.EntityFrameworkCore;
using Plantpedia.Enum;
using Plantpedia.Models;

namespace Plantpedia.DataAccessLayer.Data.Seed;

public static class SeedAccount
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserAccount>()
            .HasData(
                new UserAccount
                {
                    UserId = 1L,
                    FullName = "Nguyen Minh A",
                    Gender = 'M',
                    DateOfBirth = new DateOnly(2004, 6, 21),
                    AvatarUrl =
                        "https://www.ibm.com/content/dam/adobe-cms/instana/media_logo/AWS-EC2.png/_jcr_content/renditions/cq5dam.web.1280.1280.png",
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new UserAccount
                {
                    UserId = 2L,
                    FullName = "Nguyen Minh B",
                    Gender = 'M',
                    DateOfBirth = new DateOnly(2004, 6, 21),
                    AvatarUrl =
                        "https://tse3.mm.bing.net/th/id/OIP.JMspq1z3Vm2m00ioNzUtEgHaHa?cb=12&rs=1&pid=ImgDetMain&o=7&rm=3",
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                }
            );

        modelBuilder
            .Entity<UserLoginData>()
            .HasData(
                new UserLoginData
                {
                    UserId = 1L,
                    Username = "minha",
                    Email = "winhtuan.dev@gmail.com",
                    Role = Role.admin,
                    PasswordSalt = Convert.FromBase64String("5W8Ubef8XcxAeznr0uPnWA=="),
                    PasswordHash = Convert.FromBase64String(
                        "dD8tZsGrCCpE6ZJgyiv7s85HAfs6MI0L8ccPVZ6gOXQ="
                    ),
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EmailConfirmed = false,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                },
                new UserLoginData
                {
                    UserId = 2L,
                    Username = "minhb",
                    Email = "winhtuan@gmail.com",
                    Role = Role.user,
                    PasswordSalt = Convert.FromBase64String("5W8Ubef8XcxAeznr0uPnWA=="),
                    PasswordHash = Convert.FromBase64String(
                        "dD8tZsGrCCpE6ZJgyiv7s85HAfs6MI0L8ccPVZ6gOXQ="
                    ),
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EmailConfirmed = false,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                }
            );
    }
}
