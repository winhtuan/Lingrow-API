namespace Lingrow.BusinessLogicLayer.Options;

public class AwsOptions
{
    public string Region { get; set; } = "ap-southeast-1";
    public string BucketName { get; set; } = default!;
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
}
