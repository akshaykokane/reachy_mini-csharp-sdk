using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ReachyMini.Sdk.Configuration;
using ReachyMini.Sdk.Models;

namespace ReachyMini.Sdk.Tests.Clients;

public class DaemonClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly ReachyMiniOptions _options;

    public DaemonClientTests()
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
    public async Task GetStatusAsync_ReturnsValidStatus()
    {
        // Arrange
        var expectedStatus = new DaemonStatus
        {
            RobotName = "ReachyMini-Test",
            State = DaemonState.Running,
            WirelessVersion = true,
            DesktopAppDaemon = false,
            SimulationEnabled = false,
            BackendStatus = null,
            Version = "1.0.0"
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedStatus, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower
        });

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().Contains("/api/daemon/status")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Daemon.GetStatusAsync();

        // Assert
        result.Should().NotBeNull();
        result.RobotName.Should().Be("ReachyMini-Test");
        result.State.Should().Be(DaemonState.Running);
        result.WirelessVersion.Should().BeTrue();
        result.Version.Should().Be("1.0.0");
    }

    [Fact]
    public async Task StartAsync_WithWakeUp_SendsCorrectRequest()
    {
        // Arrange
        var expectedResponse = new Dictionary<string, string> { { "status", "started" } };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedResponse);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Daemon.StartAsync(wakeUp: true);

        // Assert
        result.Should().ContainKey("status");
        result["status"].Should().Be("started");
        
        // Verify correct endpoint was called
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri!.ToString().Contains("/api/daemon/start?wake_up=")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task StopAsync_WithSleep_SendsCorrectRequest()
    {
        // Arrange
        var expectedResponse = new Dictionary<string, string> { { "status", "stopped" } };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedResponse);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Daemon.StopAsync(gotoSleep: true);

        // Assert
        result.Should().ContainKey("status");
        result["status"].Should().Be("stopped");
        
        // Verify correct endpoint was called
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri!.ToString().Contains("/api/daemon/stop?goto_sleep=")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task RestartAsync_SendsCorrectRequest()
    {
        // Arrange
        var expectedResponse = new Dictionary<string, string> { { "status", "restarted" } };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedResponse);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString().Contains("/api/daemon/restart")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Daemon.RestartAsync();

        // Assert
        result.Should().ContainKey("status");
        result["status"].Should().Be("restarted");
    }
}
