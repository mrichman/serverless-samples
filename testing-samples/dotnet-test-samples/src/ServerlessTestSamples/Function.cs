using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using System;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ServerlessTestSamples
{
    public class Function
    {
        private readonly AmazonS3Client _s3Client;

        public Function() : this(new AmazonS3Client())
        {
        }

        internal Function(AmazonS3Client client)
        {
            this._s3Client = client;
        }

        public async Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            try
            {
                var buckets = await this._s3Client.ListBucketsAsync();
                
                if (buckets.HttpStatusCode != HttpStatusCode.OK)
                {
                    return new APIGatewayProxyResponse
                    {
                        Body = "[]",
                        StatusCode = 500,
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }

                var bucketList = buckets.Buckets.Select(p => p.BucketName);

                Console.WriteLine("Write to logs");

                return new APIGatewayProxyResponse
                {
                    Body = JsonSerializer.Serialize(bucketList),
                    StatusCode = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (AmazonS3Exception e)
            {
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                
                return new APIGatewayProxyResponse
                {
                    Body = "[]",
                    StatusCode = 500,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }
    }
}