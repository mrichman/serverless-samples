using FluentAssertions;

namespace ServerlessTestSamples.IntegrationTest;

public class IntegrationTest : IClassFixture<Setup>
{
    private string ApiUrl;
    private HttpClient _httpClient;

    public IntegrationTest()
    {
        this._httpClient = new HttpClient();
    }

    [Fact]
    public async Task TestApiGateway_ShouldReturnSuccess()
    {
        var result = await this._httpClient.GetAsync(Setup.ApiUrl);
        result.IsSuccessStatusCode.Should().BeTrue();
    }
}