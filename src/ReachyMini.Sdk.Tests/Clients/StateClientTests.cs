using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ReachyMini.Sdk.Configuration;
using ReachyMini.Sdk.Models;

namespace ReachyMini.Sdk.Tests.Clients;

public class StateClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly ReachyMiniOptions _options;

    public StateClientTests()
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
    public async Task GetBodyYawAsync_ReturnsDoubleValue()
    {
        // Arrange
        var expectedYaw = 0.523; // ~30 degrees in radians
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedYaw);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().Contains("/api/state/present_body_yaw")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.State.GetBodyYawAsync();

        // Assert
        result.Should().BeApproximately(expectedYaw, 0.001);
    }

    [Fact]
    public async Task GetAntennaJointPositionsAsync_ReturnsArrayOfTwo()
    {
        // Arrange
        var expectedPositions = new[] { 0.1, -0.1 };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedPositions);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().Contains("/api/state/present_antenna_joint_positions")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.State.GetAntennaJointPositionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(0.1, 0.001);
        result[1].Should().BeApproximately(-0.1, 0.001);
    }

    [Fact]
    public async Task GetFullStateAsync_ReturnsCompleteState()
    {
        // Arrange
        var expectedState = new FullState
        {
            ControlMode = MotorControlMode.Enabled,
            BodyYaw = 0.0,
            AntennasPosition = new[] { 0.0, 0.0 },
            Timestamp = DateTime.UtcNow
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(expectedState, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower
        });

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().Contains("/api/state/full")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });

        var client = new ReachyMiniClient(_httpClient, Options.Create(_options));

        // Act
        var result = await client.State.GetFullStateAsync();

        // Assert
        result.Should().NotBeNull();
        result.ControlMode.Should().Be(MotorControlMode.Enabled);
        result.BodyYaw.Should().Be(0.0);
        result.AntennasPosition.Should().HaveCount(2);
    }
}
