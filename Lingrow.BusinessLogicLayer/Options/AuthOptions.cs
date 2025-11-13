namespace Lingrow.BusinessLogicLayer.Options;

public class AuthOptions
{
    public int MaxFailedAccess { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 5;
}
