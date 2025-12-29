using FluentAssertions;
using ReachyMini.Sdk.Configuration;

namespace ReachyMini.Sdk.Tests.Configuration;

public class ReachyMiniOptionsTests
{
    [Fact]
    public void ReachyMiniOptions_HasCorrectDefaults()
    {
        // Arrange & Act
        var options = new ReachyMiniOptions();

        // Assert
        options.BaseUrl.Should().Be("http://localhost:8080");
        options.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        options.ThrowOnError.Should().BeTrue();
        options.RetryCount.Should().Be(3);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ReachyMiniOptions_CanBeConfigured()
    {
        // Arrange & Act
        var options = new ReachyMiniOptions
        {
            BaseUrl = "http://192.168.1.100:8080",
            Timeout = TimeSpan.FromSeconds(60),
            ThrowOnError = false,
            RetryCount = 5,
            RetryDelay = TimeSpan.FromSeconds(2)
        };

        // Assert
        options.BaseUrl.Should().Be("http://192.168.1.100:8080");
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        options.ThrowOnError.Should().BeFalse();
        options.RetryCount.Should().Be(5);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(2));
    }
}
