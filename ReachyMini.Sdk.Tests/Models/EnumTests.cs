using FluentAssertions;
using ReachyMini.Sdk.Models;

namespace ReachyMini.Sdk.Tests.Models;

public class EnumTests
{
    [Theory]
    [InlineData(MotorControlMode.Enabled)]
    [InlineData(MotorControlMode.Disabled)]
    [InlineData(MotorControlMode.GravityCompensation)]
    public void MotorControlMode_AllValuesValid(MotorControlMode mode)
    {
        // Assert
        Enum.IsDefined(typeof(MotorControlMode), mode).Should().BeTrue();
    }

    [Theory]
    [InlineData(InterpolationMode.Linear)]
    [InlineData(InterpolationMode.Minjerk)]
    [InlineData(InterpolationMode.Ease)]
    [InlineData(InterpolationMode.Cartoon)]
    public void InterpolationMode_AllValuesValid(InterpolationMode mode)
    {
        // Assert
        Enum.IsDefined(typeof(InterpolationMode), mode).Should().BeTrue();
    }

    [Theory]
    [InlineData(DaemonState.NotInitialized)]
    [InlineData(DaemonState.Starting)]
    [InlineData(DaemonState.Running)]
    [InlineData(DaemonState.Stopping)]
    [InlineData(DaemonState.Stopped)]
    [InlineData(DaemonState.Error)]
    public void DaemonState_AllValuesValid(DaemonState state)
    {
        // Assert
        Enum.IsDefined(typeof(DaemonState), state).Should().BeTrue();
    }

    [Theory]
    [InlineData(AppState.Starting)]
    [InlineData(AppState.Running)]
    [InlineData(AppState.Done)]
    [InlineData(AppState.Stopping)]
    [InlineData(AppState.Error)]
    public void AppState_AllValuesValid(AppState state)
    {
        // Assert
        Enum.IsDefined(typeof(AppState), state).Should().BeTrue();
    }

    [Theory]
    [InlineData(SourceKind.HfSpace)]
    [InlineData(SourceKind.DashboardSelection)]
    [InlineData(SourceKind.Local)]
    [InlineData(SourceKind.Installed)]
    public void SourceKind_AllValuesValid(SourceKind kind)
    {
        // Assert
        Enum.IsDefined(typeof(SourceKind), kind).Should().BeTrue();
    }

    [Theory]
    [InlineData(JobStatus.Pending)]
    [InlineData(JobStatus.InProgress)]
    [InlineData(JobStatus.Done)]
    [InlineData(JobStatus.Failed)]
    public void JobStatus_AllValuesValid(JobStatus status)
    {
        // Assert
        Enum.IsDefined(typeof(JobStatus), status).Should().BeTrue();
    }
}
