using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ReachyMini.Sdk.Configuration;
using ReachyMini.Sdk.Exceptions;

namespace ReachyMini.Sdk.Tests.Exceptions;

public class ExceptionHandlingTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly ReachyMiniOptions _options;

    public ExceptionHandlingTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:8080")
        };
        _options = new ReachyMiniOptions
        {
            BaseUrl = "http://localhost:8080",
            ThrowOnError = true
        };
    }

    [Fact]
    public async Task ApiCall_WhenServerReturns404_ThrowsReachyMiniApiException()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Resource not found")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReachyMiniApiException>(
            async () => await client.Daemon.GetStatusAsync());

        exception.StatusCode.Should().Be(404);
        exception.ResponseContent.Should().Contain("Resource not found");
    }

    [Fact]
    public async Task ApiCall_WhenServerReturns500_ThrowsReachyMiniApiException()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ReachyMiniApiException>(
            async () => await client.Daemon.GetStatusAsync());

        exception.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ApiCall_WhenConnectionFails_ThrowsRobotNotAvailableException()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act & Assert
        await Assert.ThrowsAsync<RobotNotAvailableException>(
            async () => await client.Daemon.GetStatusAsync());
    }

    [Fact]
    public async Task ApiCall_WithThrowOnErrorFalse_DoesNotThrowOnError()
    {
        // Arrange
        var optionsNoThrow = new ReachyMiniOptions
        {
            BaseUrl = "http://localhost:8080",
            ThrowOnError = false
        };

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad request")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(optionsNoThrow));

        // Act
        var result = await client.Daemon.GetStatusAsync();

        // Assert
        result.Should().BeNull();
    }
}
