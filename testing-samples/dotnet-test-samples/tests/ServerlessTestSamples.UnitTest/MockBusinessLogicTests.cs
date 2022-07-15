using System.Net;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.XRay.Recorder.Core;
using FluentAssertions;
using Moq;
using ServerlessTestSamples.Core;
using ServerlessTestSamples.Core.Services;

namespace ServerlessTestSamples.UnitTest;

public class MockBusinessLogicTests
{
    public MockBusinessLogicTests()
    {
    }
    
    [Fact]
    public async Task TestCoreBusinessLogicWithSuccessfulResponse_ShouldReturnStorageAreas()
    {
        var mockStorageService = new Mock<IStorageService>();
        
        mockStorageService.Setup(p => p.ListStorageAreas(It.IsAny<string>())).ReturnsAsync(new List<string>()
        {
            "bucket1",
            "bucket2",
            "bucket3"
        });
        
        var queryHandler = new ListStorageAreasQueryHandler(mockStorageService.Object);

        var queryResult = await queryHandler.Handle(new ListStorageAreasQuery()
        {
            FilterPrefix = string.Empty
        });

        queryResult.StorageAreas.Count().Should().Be(3);
        queryResult.StorageAreas.FirstOrDefault().Should().Be("bucket1");
    }
    
    [Fact]
    public async Task TestCoreBusinessLogicWithNullStorageResponseResponse_ShouldReturnEmptyList()
    {
        var mockStorageService = new Mock<IStorageService>();
        
        mockStorageService.Setup(p => p.ListStorageAreas(It.IsAny<string>())).ReturnsAsync(() => null);
        
        var queryHandler = new ListStorageAreasQueryHandler(mockStorageService.Object);

        var queryResult = await queryHandler.Handle(new ListStorageAreasQuery()
        {
            FilterPrefix = string.Empty
        });

        queryResult.StorageAreas.Count().Should().Be(0);
    }
    
    [Fact]
    public async Task TestCoreBusinessLogicWithStorageServiceException_ShouldReturnEmptyList()
    {
        var mockStorageService = new Mock<IStorageService>();
        
        mockStorageService.Setup(p => p.ListStorageAreas(It.IsAny<string>())).ThrowsAsync(new Exception());
        
        var queryHandler = new ListStorageAreasQueryHandler(mockStorageService.Object);

        var queryResult = await queryHandler.Handle(new ListStorageAreasQuery()
        {
            FilterPrefix = string.Empty
        });

        queryResult.StorageAreas.Count().Should().Be(0);
    }
}