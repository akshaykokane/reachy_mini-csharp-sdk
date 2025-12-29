using System.Text.Json.Serialization;

namespace ReachyMini.Sdk.Models;

/// <summary>
/// Kinds of app source.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SourceKind
{
    HfSpace,
    DashboardSelection,
    Local,
    Installed
}

/// <summary>
/// Status of a running app.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppState
{
    Starting,
    Running,
    Done,
    Stopping,
    Error
}

/// <summary>
/// Enum representing the state of the Reachy Mini daemon.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DaemonState
{
    NotInitialized,
    Starting,
    Running,
    Stopping,
    Stopped,
    Error
}

/// <summary>
/// Enum for motor control modes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MotorControlMode
{
    Enabled,
    Disabled,
    GravityCompensation
}

/// <summary>
/// Interpolation modes for movement.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InterpolationMode
{
    Linear,
    Minjerk,
    Ease,
    Cartoon
}

/// <summary>
/// Enum for job status.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobStatus
{
    Pending,
    InProgress,
    Done,
    Failed
}
