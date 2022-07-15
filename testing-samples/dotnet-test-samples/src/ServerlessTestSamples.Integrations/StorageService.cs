using System.Net;
using Amazon.S3;
using ServerlessTestSamples.Core.Services;

namespace ServerlessTestSamples.Integrations;

public class StorageService : IStorageService
{
    private readonly AmazonS3Client _s3Client;

    public StorageService(AmazonS3Client client)
    {
        this._s3Client = client ?? new AmazonS3Client();
    }

    public async Task<IEnumerable<string>> ListStorageAreas(string filterPrefix)
    {
        var buckets = await this._s3Client.ListBucketsAsync();

        if (buckets.HttpStatusCode != HttpStatusCode.OK)
        {
            return new List<string>(0);
        }

        return buckets.Buckets.Select(p => p.BucketName);
    }
}