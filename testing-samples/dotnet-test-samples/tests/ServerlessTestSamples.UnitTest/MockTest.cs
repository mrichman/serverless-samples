using System.Net;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Moq;

namespace ServerlessTestSamples.UnitTest;

public class MockTest
{
    private bool _runningLocally = string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("LOCAL_RUN")) ? true : bool.Parse(System.Environment.GetEnvironmentVariable("LOCAL_RUN"));

    [Fact]
    public async Task TestLambdaHandlerWithValidS3Response_ShouldReturnSuccess()
    {
        var mockedS3Client = new Mock<AmazonS3Client>();
        mockedS3Client.Setup(p => p.ListBucketsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ListBucketsResponse()
        {
            Buckets = new List<S3Bucket>()
            {
                new S3Bucket(){BucketName = "bucket1"},
                new S3Bucket(){BucketName = "bucket2"},
                new S3Bucket(){BucketName = "bucket3"},
            },
            HttpStatusCode = HttpStatusCode.OK
        });
        
        var mockRequest = new Mock<APIGatewayProxyRequest>();

        var function = new Function(mockedS3Client.Object);

        var result = await function.Handler(mockRequest.Object, new TestLambdaContext());

        result.StatusCode.Should().Be(200);
        result.Body.Should().Be("[\"bucket1\",\"bucket2\",\"bucket3\"]");
    }

    [Fact]
    public async Task TestLambdaHandlerWithEmptyS3Response_ShouldReturnEmpty()
    {
        var mockedS3Client = new Mock<AmazonS3Client>();
        mockedS3Client.Setup(p => p.ListBucketsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ListBucketsResponse()
        {
            Buckets = new List<S3Bucket>()
            {
            },
            HttpStatusCode = HttpStatusCode.OK
        });
        
        var mockRequest = new Mock<APIGatewayProxyRequest>();

        var function = new Function(mockedS3Client.Object);

        var result = await function.Handler(mockRequest.Object, new TestLambdaContext());

        result.StatusCode.Should().Be(200);
        result.Body.Should().Be("[]");
    }

    [Fact]
    public async Task TestLambdaHandlerWithS3NullResponse_ShouldReturnEmpty()
    {
        var mockedS3Client = new Mock<AmazonS3Client>();
        mockedS3Client.Setup(p => p.ListBucketsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ListBucketsResponse()
        {
            Buckets = null,
            HttpStatusCode = HttpStatusCode.BadRequest
        });
        
        var mockRequest = new Mock<APIGatewayProxyRequest>();

        var function = new Function(mockedS3Client.Object);

        var result = await function.Handler(mockRequest.Object, new TestLambdaContext());

        result.StatusCode.Should().Be(500);
        result.Body.Should().Be("[]");
    }

    [Fact]
    public async Task TestLambdaHandlerWithS3Exception_ShouldReturnEmpty()
    {
        var mockedS3Client = new Mock<AmazonS3Client>();
        mockedS3Client.Setup(p => p.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Mock S3 failure"));
        
        var mockRequest = new Mock<APIGatewayProxyRequest>();

        var function = new Function(mockedS3Client.Object);

        var result = await function.Handler(mockRequest.Object, new TestLambdaContext());

        result.StatusCode.Should().Be(500);
        result.Body.Should().Be("[]");
    }
}