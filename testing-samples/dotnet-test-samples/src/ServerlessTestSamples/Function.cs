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
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Amazon.XRay.Recorder.Handlers.System.Net;
using ServerlessTestSamples.Core;
using ServerlessTestSamples.Integrations;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ServerlessTestSamples
{
    public class Function
    {
        private readonly ListStorageAreasQueryHandler _queryHandler;
        private readonly HttpClient _httpClient;

        public Function() : this(null, null)
        {
        }

        internal Function(AmazonS3Client client, HttpClient httpClient)
        {
            AWSSDKHandler.RegisterXRayForAllServices();

            this._queryHandler = new ListStorageAreasQueryHandler(new StorageService(client ?? new AmazonS3Client()));
            this._httpClient = httpClient ?? new HttpClient(new HttpClientXRayTracingHandler(new HttpClientHandler()));
        }

        public async Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            try
            {
                var queryResult = await this._queryHandler.Handle(new ListStorageAreasQuery());

                // Demonstrate tracing of an external HTTP request.
                await this._httpClient.GetAsync("https://google.com");

                Console.WriteLine("Write to logs");

                return new APIGatewayProxyResponse
                {
                    Body = JsonSerializer.Serialize(queryResult),
                    StatusCode = 200,
                    Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
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
                    Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
                };
            }
        }
    }
}