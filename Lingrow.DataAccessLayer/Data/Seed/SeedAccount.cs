using System;
using Lingrow.Enum;
using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.DataAccessLayer.Data.Seed;

public static class SeedAccount
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserAccount>()
            .HasData(
                new UserAccount
                {
                    UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CognitoSub = "sub-0001",
                    Email = "winhtuan.dev@gmail.com",
                    Username = "minha",
                    FullName = "Nguyen Minh A",
                    Gender = 'M',
                    DateOfBirth = new DateOnly(2004, 6, 21),
                    Role = Role.admin,
                    AvatarUrl =
                        "https://www.ibm.com/content/dam/adobe-cms/instana/media_logo/AWS-EC2.png/_jcr_content/renditions/cq5dam.web.1280.1280.png",
                    Status = UserStatus.Active,
                    EmailConfirmed = true,
                    EmailConfirmedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    LastLoginAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                }
            );
    }
}
