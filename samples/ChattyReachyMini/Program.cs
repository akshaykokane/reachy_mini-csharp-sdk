using ReachyMini.Sdk;
using ReachyMini.Sdk.Configuration;
using ReachyMini.Sdk.Models;
using ReachyMini.Sdk.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

// Load configuration from appsettings.json and appsettings.local.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .Build();

// Get configuration values
var azureSpeechKey = configuration["Azure:SpeechKey"] ?? throw new Exception("Azure:SpeechKey not found in appsettings.json");
var azureSpeechRegion = configuration["Azure:SpeechRegion"] ?? throw new Exception("Azure:SpeechRegion not found in appsettings.json");
var azureOpenAIEndpoint = configuration["Azure:OpenAIEndpoint"] ?? throw new Exception("Azure:OpenAIEndpoint not found in appsettings.json");
var azureOpenAIKey = configuration["Azure:OpenAIKey"] ?? throw new Exception("Azure:OpenAIKey not found in appsettings.json");
var azureOpenAIDeployment = configuration["Azure:OpenAIDeployment"] ?? throw new Exception("Azure:OpenAIDeployment not found in appsettings.json");
var reachyBaseUrl = configuration["ReachyMini:BaseUrl"] ?? "http://localhost:8080";
var recognitionLanguage = configuration["Speech:RecognitionLanguage"] ?? "en-US";
var synthesisVoice = configuration["Speech:SynthesisVoice"] ?? "en-US-AriaNeural";

Console.WriteLine("=== Chatty Reachy Mini ===");
Console.WriteLine("Voice-enabled AI assistant for Reachy Mini Robot\n");

// Initialize Reachy Mini client
var options = Options.Create(new ReachyMiniOptions
{
    BaseUrl = reachyBaseUrl,
    Timeout = TimeSpan.FromSeconds(30)
});
var httpClient = new HttpClient();
var reachyClient = new ReachyMiniClient(httpClient, options);

// Initialize Azure OpenAI Chat client
var azureOpenAIClient = new AzureOpenAIClient(
    new Uri(azureOpenAIEndpoint),
    new AzureKeyCredential(azureOpenAIKey));
var chatClient = azureOpenAIClient.GetChatClient(azureOpenAIDeployment);

// Initialize Azure Speech services
var speechConfig = SpeechConfig.FromSubscription(azureSpeechKey, azureSpeechRegion);
speechConfig.SpeechRecognitionLanguage = recognitionLanguage;
speechConfig.SpeechSynthesisVoiceName = synthesisVoice;

// System prompt for Reachy Mini personality
var systemPrompt = @"You are Reachy Mini, a friendly and helpful humanoid robot assistant. 
You have expressive antennas that move to show emotions, and you can move your head and body. 
Keep responses brief and conversational (1-2 sentences). 
Be enthusiastic, curious, and engaging. Use simple language.";

var conversationHistory = new List<ChatMessage>
{
    new SystemChatMessage(systemPrompt)
};

try
{
    // Wake up Reachy
    Console.WriteLine("Waking up Reachy Mini...");
    await reachyClient.Move.WakeUpAsync();
    await Task.Delay(2000);

    // Get robot status
    var status = await reachyClient.Daemon.GetStatusAsync();
    Console.WriteLine($"Reachy Mini '{status.RobotName}' is ready!\n");

    // Set antennas to neutral position
    var neutralPose = new GotoModelRequest
    {
        Antennas = new double[] { 0.0, 0.0 },
        Duration = 1.0,
        Interpolation = InterpolationMode.Minjerk
    };
    
    try
    {
        await reachyClient.Move.GotoAsync(neutralPose);
    }
    catch (ReachyMiniApiException ex)
    {
        Console.WriteLine($"API Error: {ex.Message}");
        Console.WriteLine($"Response Content: {ex.ResponseContent}");
        throw;
    }

    Console.WriteLine("Listening mode activated. Speak to Reachy Mini!");
    Console.WriteLine("Say 'goodbye' or 'exit' to end the conversation.\n");

    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
    using var synthesizer = new SpeechSynthesizer(speechConfig);

    bool continueConversation = true;

    while (continueConversation)
    {
        // Listen for user input
        Console.WriteLine("🎤 Listening... (speak now)");
        
        // Move antennas to "listening" position
        var listeningPose = new GotoModelRequest
        {
            Antennas = new double[] { 30, 30 },
            Duration = 0.5f,
            Interpolation = InterpolationMode.Ease
        };
        await reachyClient.Move.GotoAsync(listeningPose);

        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            var userInput = result.Text;
            Console.WriteLine($"👤 You: {userInput}");

            // Check for exit commands
            if (userInput.ToLower().Contains("goodbye") || 
                userInput.ToLower().Contains("exit") ||
                userInput.ToLower().Contains("bye"))
            {
                var farewellPose = new GotoModelRequest
                {
                    Antennas = new double[] { -20, -20 },
                    Duration = 1.0f,
                    Interpolation = InterpolationMode.Minjerk
                };
                await reachyClient.Move.GotoAsync(farewellPose);

                await synthesizer.SpeakTextAsync("Goodbye! It was nice talking with you. I'm going to sleep now.");
                continueConversation = false;
                continue;
            }

            // Add user message to conversation
            conversationHistory.Add(new UserChatMessage(userInput));

            // Get AI response
            Console.WriteLine("🤖 Reachy is thinking...");
            
            // Move antennas to "thinking" position
            var thinkingPose = new GotoModelRequest
            {
                Antennas = new double[] { 45, -45 },
                Duration = 0.3f,
                Interpolation = InterpolationMode.Linear
            };
            await reachyClient.Move.GotoAsync(thinkingPose);

            var completion = await chatClient.CompleteChatAsync(conversationHistory);
            var response = completion.Value.Content[0].Text;

            conversationHistory.Add(new AssistantChatMessage(response));

            // Keep conversation history manageable
            if (conversationHistory.Count > 15)
            {
                // Keep system message and last 12 messages
                conversationHistory = new List<ChatMessage> 
                { 
                    conversationHistory[0] 
                }.Concat(conversationHistory.Skip(conversationHistory.Count - 12)).ToList();
            }

            Console.WriteLine($"🤖 Reachy: {response}");

            // Move antennas to "speaking" position
            var speakingPose = new GotoModelRequest
            {
                Antennas = new double[] { 60, 60 },
                Duration = 0.5f,
                Interpolation = InterpolationMode.Ease
            };
            await reachyClient.Move.GotoAsync(speakingPose);

            // Speak the response
            await synthesizer.SpeakTextAsync(response);

            // Return to neutral
            await reachyClient.Move.GotoAsync(neutralPose);
            Console.WriteLine();
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            Console.WriteLine("⚠️  Speech not recognized. Please try again.\n");
            
            // Confused antenna movement
            var confusedPose = new GotoModelRequest
            {
                Antennas = new double[] { -30, 30 },
                Duration = 0.5f,
                Interpolation = InterpolationMode.Ease
            };
            await reachyClient.Move.GotoAsync(confusedPose);
            await Task.Delay(500);
            await reachyClient.Move.GotoAsync(neutralPose);
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            Console.WriteLine($"⚠️  Recognition canceled: {cancellation.Reason}");
            if (cancellation.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"Error: {cancellation.ErrorDetails}");
            }
            continueConversation = false;
        }
    }

    // Put Reachy to sleep
    Console.WriteLine("\nPutting Reachy Mini to sleep...");
    await reachyClient.Move.GotoSleepAsync();
    await Task.Delay(2000);

    Console.WriteLine("Reachy Mini is now sleeping. Goodbye! 👋");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message} {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Details: {ex.InnerException.Message}");
    }

    // Try to put robot to sleep on error
    try
    {
        await reachyClient.Move.GotoSleepAsync();
    }
    catch { /* Ignore sleep errors */ }
}
