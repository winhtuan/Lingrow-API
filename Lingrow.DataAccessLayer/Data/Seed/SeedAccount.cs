using System;
using Lingrow.Enum;
using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.DataAccessLayer.Data.Seed;

public static class SeedAccount
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        var user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

        modelBuilder
            .Entity<UserAccount>()
            .HasData(
                new UserAccount
                {
                    UserId = user1Id,
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
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new UserAccount
                {
                    UserId = user2Id,
                    CognitoSub = "sub-0002",
                    Email = "winhtuan@gmail.com",
                    Username = "minhb",
                    FullName = "Nguyen Minh B",
                    Gender = 'M',
                    DateOfBirth = new DateOnly(2004, 6, 21),
                    Role = Role.user,
                    AvatarUrl =
                        "https://tse3.mm.bing.net/th/id/OIP.JMspq1z3Vm2m00ioNzUtEgHaHa?cb=12&rs=1&pid=ImgDetMain&o=7&rm=3",
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                }
            );
    }
}
