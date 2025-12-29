# ReachyMini.Sdk Samples

This folder contains sample projects demonstrating how to use the ReachyMini.Sdk.

## Basic Usage

A simple console application showing basic SDK operations:
- Get daemon status
- Wake up robot
- Get robot state
- List apps
- Put robot to sleep

**Run:**
```bash
cd samples/BasicUsage
dotnet run
```

## Web API Sample

An ASP.NET Core minimal API that wraps the ReachyMini.Sdk, demonstrating:
- Dependency injection configuration
- Configuration from appsettings.json
- RESTful API endpoints
- Swagger documentation

**Run:**
```bash
cd samples/WebApiSample
dotnet run
```

Then open: http://localhost:5000/swagger

**API Endpoints:**
- `GET /api/status` - Get daemon status
- `POST /api/robot/wakeup` - Wake up robot
- `POST /api/robot/sleep` - Put robot to sleep
- `GET /api/robot/state` - Get full robot state
- `POST /api/robot/goto` - Move robot to position
- `GET /api/apps` - List installed apps
- `POST /api/daemon/start` - Start daemon
- `POST /api/daemon/stop` - Stop daemon

## Chatty Reachy Mini

A voice-enabled AI assistant that makes Reachy Mini interactive! Features:
- 🎤 Voice recognition (Azure Speech)
- 🤖 AI conversations (Azure OpenAI GPT)
- 🔊 Text-to-speech responses
- 📡 Expressive antenna movements
- 💬 Conversational memory

**Requirements:**
- Azure Cognitive Services Speech key
- Azure OpenAI service with deployed model

**Run:**
```bash
cd samples/ChattyReachyMini
# Configure API keys in Program.cs first
dotnet run
```

See [ChattyReachyMini/README.md](ChattyReachyMini/README.md) for detailed setup instructions.

## Prerequisites

- .NET 9.0 SDK
- Reachy Mini robot running on localhost:8080 (or update configuration)

## Configuration

Update the robot URL in:
- **BasicUsage**: `Program.cs` line 7
- **WebApiSample**: `appsettings.json` ReachyMini:BaseUrl
- **ChattyReachyMini**: `Program.cs` constants or environment variables
