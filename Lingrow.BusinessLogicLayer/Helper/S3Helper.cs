using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Lingrow.BusinessLogicLayer.Options;
using Microsoft.Extensions.Options;

namespace Lingrow.BusinessLogicLayer.Helper;

public class S3Helper
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsOptions _options;

    public S3Helper(IOptions<AwsOptions> awsOptions)
    {
        _options = awsOptions.Value;

        _s3Client = new AmazonS3Client(
            _options.AccessKey,
            _options.SecretKey,
            RegionEndpoint.GetBySystemName(_options.Region)
        );
    }

    /// <summary>
    /// Tạo URL tạm thời (Presigned URL) để tải ảnh (GET)
    /// </summary>
    public string GetPresignedGetUrl(string key, int expiresInMinutes = 60)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
        };

        return _s3Client.GetPreSignedURL(request);
    }

    /// <summary>
    /// Tạo URL tạm thời để upload ảnh (PUT)
    /// </summary>
    public string GetPresignedPutUrl(
        string key,
        string contentType = "image/webp",
        int expiresInMinutes = 15
    )
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
        };

        return _s3Client.GetPreSignedURL(request);
    }

    /// <summary>
    /// Sinh key chuẩn cho avatar theo userId (vd: user-avatar/avatar-123.webp)
    /// </summary>
    public static string GetAvatarKey(Guid userId, string extension = "webp") =>
        $"user-avatar/avatar-{userId}.{extension}";
}
