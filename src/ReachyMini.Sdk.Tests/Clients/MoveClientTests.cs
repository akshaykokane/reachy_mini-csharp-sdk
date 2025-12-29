using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ReachyMini.Sdk.Configuration;
using ReachyMini.Sdk.Models;

namespace ReachyMini.Sdk.Tests.Clients;

public class MoveClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly ReachyMiniOptions _options;

    public MoveClientTests()
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
    public async Task WakeUpAsync_ReturnsMoveUUID()
    {
        // Arrange
        var expectedUuid = Guid.NewGuid();
        var expectedResponse = new MoveUUID { Uuid = expectedUuid };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedResponse);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString().Contains("/api/move/play/wake_up")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Move.WakeUpAsync();

        // Assert
        result.Should().NotBeNull();
        result.Uuid.Should().Be(expectedUuid);
    }

    [Fact]
    public async Task GotoSleepAsync_ReturnsMoveUUID()
    {
        // Arrange
        var expectedUuid = Guid.NewGuid();
        var expectedResponse = new MoveUUID { Uuid = expectedUuid };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedResponse);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString().Contains("/api/move/play/goto_sleep")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Move.GotoSleepAsync();

        // Assert
        result.Should().NotBeNull();
        result.Uuid.Should().Be(expectedUuid);
    }

    [Fact]
    public async Task GotoAsync_WithValidRequest_ReturnsMoveUUID()
    {
        // Arrange
        var expectedUuid = Guid.NewGuid();
        var gotoRequest = new GotoModelRequest
        {
            HeadPose = new XYZRPYPose { X = 0.2, Y = 0, Z = 0.3, Roll = 0, Pitch = 0, Yaw = 0 },
            Antennas = new[] { 0.0, 0.0 },
            BodyYaw = 0.0,
            Duration = 2.0,
            Interpolation = InterpolationMode.Minjerk
        };

        var expectedResponse = new MoveUUID { Uuid = expectedUuid };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedResponse);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString().Contains("/api/move/goto")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Move.GotoAsync(gotoRequest);

        // Assert
        result.Should().NotBeNull();
        result.Uuid.Should().Be(expectedUuid);
    }

    [Fact]
    public async Task GetRunningMovesAsync_ReturnsListOfMoves()
    {
        // Arrange
        var expectedMoves = new List<MoveUUID>
        {
            new() { Uuid = Guid.NewGuid() },
            new() { Uuid = Guid.NewGuid() }
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedMoves);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().Contains("/api/move/running")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Move.GetRunningMovesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task StopMoveAsync_StopsSpecificMove()
    {
        // Arrange
        var moveUuid = new MoveUUID { Uuid = Guid.NewGuid() };
        var expectedResponse = new Dictionary<string, string> { { "status", "stopped" } };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedResponse);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString().Contains("/api/move/stop")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Move.StopMoveAsync(moveUuid);

        // Assert
        result.Should().ContainKey("status");
        result["status"].Should().Be("stopped");
    }

    [Fact]
    public async Task ListRecordedMovesAsync_ReturnsListOfMoveNames()
    {
        // Arrange
        var expectedMoves = new List<string> { "wave", "nod", "look_around" };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedMoves);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().Contains("/api/move/recorded-move-datasets/list/dataset1")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.Move.ListRecordedMovesAsync("dataset1");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("wave");
        result.Should().Contain("nod");
        result.Should().HaveCount(3);
    }
}
