using FluentAssertions;
using ReachyMini.Sdk.Models;

namespace ReachyMini.Sdk.Tests.Models;

public class MovementModelsTests
{
    [Fact]
    public void XYZRPYPose_DefaultValues_AreZero()
    {
        // Arrange & Act
        var pose = new XYZRPYPose();

        // Assert
        pose.X.Should().Be(0.0);
        pose.Y.Should().Be(0.0);
        pose.Z.Should().Be(0.0);
        pose.Roll.Should().Be(0.0);
        pose.Pitch.Should().Be(0.0);
        pose.Yaw.Should().Be(0.0);
    }

    [Fact]
    public void XYZRPYPose_CanBeSet()
    {
        // Arrange & Act
        var pose = new XYZRPYPose
        {
            X = 0.2,
            Y = 0.1,
            Z = 0.3,
            Roll = 0.1,
            Pitch = 0.2,
            Yaw = 0.3
        };

        // Assert
        pose.X.Should().Be(0.2);
        pose.Y.Should().Be(0.1);
        pose.Z.Should().Be(0.3);
        pose.Roll.Should().Be(0.1);
        pose.Pitch.Should().Be(0.2);
        pose.Yaw.Should().Be(0.3);
    }

    [Fact]
    public void GotoModelRequest_RequiresDuration()
    {
        // Arrange & Act
        var request = new GotoModelRequest
        {
            Duration = 2.0,
            Interpolation = InterpolationMode.Minjerk
        };

        // Assert
        request.Duration.Should().Be(2.0);
        request.Interpolation.Should().Be(InterpolationMode.Minjerk);
    }

    [Fact]
    public void GotoModelRequest_InterpolationDefaultsToMinjerk()
    {
        // Arrange & Act
        var request = new GotoModelRequest
        {
            Duration = 2.0
        };

        // Assert
        request.Interpolation.Should().Be(InterpolationMode.Minjerk);
    }

    [Fact]
    public void FullBodyTarget_AllFieldsOptional()
    {
        // Arrange & Act
        var target = new FullBodyTarget();

        // Assert
        target.TargetHeadPose.Should().BeNull();
        target.TargetAntennas.Should().BeNull();
        target.TargetBodyYaw.Should().BeNull();
        target.Timestamp.Should().BeNull();
    }

    [Fact]
    public void FullBodyTarget_CanSetAllFields()
    {
        // Arrange
        var headPose = new XYZRPYPose { X = 0.2, Y = 0, Z = 0.3 };
        var antennas = new[] { 0.1, -0.1 };
        var bodyYaw = 0.523;
        var timestamp = DateTime.UtcNow;

        // Act
        var target = new FullBodyTarget
        {
            TargetHeadPose = headPose,
            TargetAntennas = antennas,
            TargetBodyYaw = bodyYaw,
            Timestamp = timestamp
        };

        // Assert
        target.TargetHeadPose.Should().Be(headPose);
        target.TargetAntennas.Should().Equal(antennas);
        target.TargetBodyYaw.Should().Be(bodyYaw);
        target.Timestamp.Should().Be(timestamp);
    }

    [Fact]
    public void MoveUUID_StoresGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var moveUuid = new MoveUUID { Uuid = guid };

        // Assert
        moveUuid.Uuid.Should().Be(guid);
    }

    [Fact]
    public void Matrix4x4Pose_Requires16Elements()
    {
        // Arrange
        var matrix = new double[16];
        for (int i = 0; i < 16; i++)
        {
            matrix[i] = i;
        }

        // Act
        var pose = new Matrix4x4Pose { M = matrix };

        // Assert
        pose.M.Should().HaveCount(16);
        pose.M[0].Should().Be(0);
        pose.M[15].Should().Be(15);
    }
}
