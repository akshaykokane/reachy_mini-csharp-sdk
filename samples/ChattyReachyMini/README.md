# Chatty Reachy Mini

A voice-enabled AI assistant application that makes Reachy Mini interactive! The robot can listen to voice commands, process them using OpenAI GPT, respond with synthesized speech, and express emotions through antenna movements.

## Features

- 🎤 **Voice Recognition** - Uses Azure Cognitive Services Speech-to-Text
- 🤖 **AI Conversations** - Powered by OpenAI GPT-4o-mini
- 🔊 **Text-to-Speech** - Natural voice responses via Azure Speech Synthesis
- 📡 **Expressive Antennas** - Robot shows emotions through antenna movements:
  - Listening: Antennas up
  - Thinking: Antennas asymmetric
  - Speaking: Antennas fully raised
  - Confused: Antennas at different angles
- 💬 **Conversational Memory** - Maintains context throughout the conversation

## Prerequisites

1. **Reachy Mini Robot** running on localhost:8080
2. **Azure Cognitive Services** account for Speech services
3. **Azure OpenAI** service with deployed model

## Setup

### 1. Get Azure Speech Services Key

1. Go to [Azure Portal](https://portal.azure.com)
2. Create a "Speech Services" resource
3. Copy the **Key** and **Region**

### 2. Get Azure OpenAI Access

1. Go to [Azure Portal](https://portal.azure.com)
2. Create an "Azure OpenAI" resource
3. Deploy a model (e.g., gpt-4o-mini or gpt-4o)
4. Copy the **Endpoint**, **Key**, and **Deployment Name**

### 3. Configure the Application

Create a `appsettings.local.json` file in the `ChattyReachyMini` folder:

```json
{
  "Azure": {
    "SpeechKey": "YOUR_AZURE_SPEECH_KEY",
    "SpeechRegion": "YOUR_REGION",
    "OpenAIEndpoint": "https://YOUR_RESOURCE_NAME.openai.azure.com/",
    "OpenAIKey": "YOUR_AZURE_OPENAI_KEY",
    "OpenAIDeployment": "YOUR_DEPLOYMENT_NAME"
  },
  "ReachyMini": {
    "BaseUrl": "http://localhost:8080"
  },
  "Speech": {
    "RecognitionLanguage": "en-US",
    "SynthesisVoice": "en-US-AriaNeural"
  }
}
```

Replace the placeholder values with your actual Azure credentials:
- `SpeechKey`: Your Azure Speech Services key
- `SpeechRegion`: Your Azure region (e.g., "eastus", "westus")
- `OpenAIEndpoint`: Your Azure OpenAI endpoint URL
- `OpenAIKey`: Your Azure OpenAI API key
- `OpenAIDeployment`: Your deployed model name (e.g., "gpt-4o-mini", "gpt-4o")

> **Note:** The `appsettings.local.json` file is git-ignored to protect your secrets. Never commit API keys to source control!

## Run

```bash
cd samples/ChattyReachyMini
dotnet run
```

## Usage

1. The application will wake up Reachy Mini
2. Speak naturally when prompted
3. Reachy will:
   - Move antennas to show it's listening
   - Process your speech
   - Think (shown by antenna movement)
   - Respond with voice and expressive movements
4. Say "goodbye" or "exit" to end the conversation

## Example Conversation

```
🎤 Listening... (speak now)
👤 You: Hello Reachy! How are you today?
🤖 Reachy is thinking...
🤖 Reachy: Hi there! I'm doing great, thank you for asking! I'm excited to chat with you today.

🎤 Listening... (speak now)
👤 You: Can you tell me about yourself?
🤖 Reachy is thinking...
🤖 Reachy: I'm Reachy Mini, a friendly humanoid robot! I have expressive antennas and love helping people.

🎤 Listening... (speak now)
👤 You: Goodbye Reachy
🤖 Reachy: Goodbye! It was nice talking with you. I'm going to sleep now.
```

## Customization

### Change Voice

Modify the voice in `appsettings.local.json`:

```json
{
  "Speech": {
    "SynthesisVoice": "en-US-JennyNeural"
  }
}
```

Popular voices:
- `"en-US-AriaNeural"` - Female (default)
- `"en-US-JennyNeural"` - Female
- `"en-US-GuyNeural"` - Male

[Browse all voices](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=tts)

### Change Language

Update your `appsettings.local.json`:

```json
{
  "Speech": {
    "RecognitionLanguage": "es-ES",
### Change AI Model

Deploy a different model in Azure OpenAI and update the deployment name in `appsettings.local.json`:

```json
{
  "Azure": {
    "OpenAIDeployment": "gpt-4o"
  }
}
```

Available models:
- `"gpt-4o-mini"` - Faster, cheaper (recommended)
- `"gpt-4o"` - More capable, slower, more expensive
- `"gpt-35-turbo"` - Even faster and cheaper

Edit the `systemPrompt` variable to change Reachy's personality:

```csharp
var systemPrompt = @"You are Reachy Mini, a witty and sarcastic robot...";
```

### Change AI Model

Deploy a different model in Azure OpenAI and update the deployment name:
- `"gpt-4o"` - More capable, slower, more expensive
- `"gpt-4o-mini"` - Faster, cheaper
- `"gpt-35-turbo"` - Even faster and cheaper

## Cost Considerations

- **Azure Speech**: ~$1 per hour of audio processed
- **Azure OpenAI GPT-4o-mini**: Pay-as-you-go pricing based on tokens
- **Azure OpenAI GPT-4o**: Higher per-token cost

Free tiers and credits available for new Azure accounts!

## Troubleshooting

**Microphone not detected:**
- Ensure microphone permissions are granted
- Check default microphone in system settings

**Speech recognition fails:**
- Verify Azure Speech key and region
- Check internet connection
- Speak clearly and reduce background noise

## Privacy & Security

⚠️ **Never commit API keys to source control!**

- Keep your credentials in `appsettings.local.json` (git-ignored)
- The `appsettings.json` file contains only placeholder values
- For production deployments, use Azure Key Vault or environment variables
- Verify Azure OpenAI endpoint, key, and deployment name
- Check that the model deployment is active
- Ensure proper internet connection
- Verify Azure subscription has sufficient credits

## Privacy & Security

⚠️ **Never commit API keys to source control!**

Use environment variables or Azure Key Vault for production deployments.
