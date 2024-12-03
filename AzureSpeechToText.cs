using Microsoft.CognitiveServices.Speech; // Azure Speech SDK for speech recognition.
using Microsoft.CognitiveServices.Speech.Audio; // For handling audio input.

// Custom namespace for project-specific functionality.

namespace ZoharBible
{
    /// <summary>
    /// Handles transcription of audio files and keyword-based command execution using Azure Cognitive Services.
    /// </summary>
    public class AzureSpeechToText
    {
        /// <summary>
        /// Azure subscription key for the Speech Service.
        /// </summary>
        private static string subscriptionKey = Secrets.subscriptionKey;

        /// <summary>
        /// Azure region where the Speech Service is deployed.
        /// </summary>
        private static string serviceRegion = Secrets.azureRegion;

        /// <summary>
        /// Transcribes audio from a file, processes recognized text, and executes commands based on keywords.
        /// </summary>
        /// <param name="filepath">Path to the WAV file to be transcribed.</param>
        /// <param name="keyword">List of keywords to be detected in the transcription.</param>
        /// <returns>A string summarizing the outcome of the transcription and command processing.</returns>
        public static async Task<string> TranscribeAudioAsync(string filepath, List<string> keyword)
        {
            // Inform the user that processing has started.
            string dSentiment = await Translator.TranslateTextToGiven("Working on your command. Patience please.");
            await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);

            // Configure Azure Speech Service with subscription details.
            var config = SpeechConfig.FromSubscription(Secrets.wToTSubscription, Secrets.llocation);

            // Setup audio input from the provided file.
            using var audioInput = AudioConfig.FromWavFileInput(filepath);
            using var recognizer = new SpeechRecognizer(config, audioInput);

            // Recognize speech asynchronously.
            var result = await recognizer.RecognizeOnceAsync();

            // Flag to track if any keyword matches are found.
            bool isInKeywords = false;

            // Check if any of the provided keywords are present in the transcription.
            foreach (var item in keyword)
            {
                if (result.Text.Contains(item))
                    isInKeywords = true;
            }

            // Process recognized speech if a match is found.
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                if (isInKeywords) // If the transcription contains any of the keywords.
                {
                    // Handle specific commands based on recognized content.
                    if (result.Text.Contains("Elon", StringComparison.OrdinalIgnoreCase) ||
                        result.Text.Contains("X", StringComparison.OrdinalIgnoreCase) ||
                        result.Text.Contains("Grok", StringComparison.OrdinalIgnoreCase))
                    {
                        dSentiment = await Translator.TranslateTextToGiven("Elon is busy working on your text.");
                        await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);

                        // Fetch additional information from a custom API or service.
                        dSentiment = await Secrets.GetGrok(result.Text);

                        // Clean and process the result text.
                        string cleanedText = GlobalVars.DialogueCleaner(dSentiment);
                        cleanedText = await Translator.TranslateTextToGiven(cleanedText);
                        GlobalVars.AIInteractiveText = cleanedText;

                        // Provide spoken feedback if AI interactivity is disabled.
                        if (!GlobalVars.AIInteractive)
                        {
                            await GlobalVars.ttsService.ConvertTextToSpeechAsync(cleanedText);
                            await GlobalVars.ttsService.StopSpeakingAsync();
                        }

                        return "Talked Elon.";
                    }

                    // Handle commands related to Bill/Microsoft.
                    if (result.Text.Contains("Bill", StringComparison.OrdinalIgnoreCase) ||
                        result.Text.Contains("Microsoft", StringComparison.OrdinalIgnoreCase))
                    {
                        dSentiment = await Translator.TranslateTextToGiven("Bill is busy working on your text.");
                        await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);

                        // Call an API endpoint and process the response.
                        dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                            Secrets.RESTAPI + @"ChatGPT/" + result.Text);

                        string cleanedText = GlobalVars.DialogueCleaner(dSentiment);
                        cleanedText = await Translator.TranslateTextToGiven(cleanedText);
                        GlobalVars.AIInteractiveText = cleanedText;

                        if (!GlobalVars.AIInteractive)
                        {
                            await GlobalVars.ttsService.ConvertTextToSpeechAsync(cleanedText);
                            await GlobalVars.ttsService.StopSpeakingAsync();
                        }

                        return "Talked Bill.";
                    }

                    // Handle commands related to Google/Gemini.
                    if (result.Text.Contains("Google", StringComparison.OrdinalIgnoreCase) ||
                        result.Text.Contains("Gemini", StringComparison.OrdinalIgnoreCase))
                    {
                        dSentiment = await Translator.TranslateTextToGiven("Google is busy working on your text.");
                        await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);

                        dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                            Secrets.RESTAPI + @"Google/" + result.Text);

                        string cleanedText = GlobalVars.DialogueCleaner(dSentiment);
                        cleanedText = await Translator.TranslateTextToGiven(cleanedText);
                        GlobalVars.AIInteractiveText = cleanedText;

                        if (!GlobalVars.AIInteractive)
                        {
                            await GlobalVars.ttsService.ConvertTextToSpeechAsync(cleanedText);
                            await GlobalVars.ttsService.StopSpeakingAsync();
                        }

                        return "Talked Gemini.";
                    }

                    // Handle commands to activate or deactivate AI dialogue.
                    if (result.Text.Contains("Dialogue", StringComparison.OrdinalIgnoreCase))
                    {
                        if (result.Text.Contains("Activate", StringComparison.OrdinalIgnoreCase) ||
                            result.Text.Contains("Start", StringComparison.OrdinalIgnoreCase))
                        {
                            GlobalVars._Dialogue = true;
                            return "AI Dialogue checkbox will be started.";
                        }

                        if (result.Text.Contains("Deactivate", StringComparison.OrdinalIgnoreCase) ||
                            result.Text.Contains("Stop", StringComparison.OrdinalIgnoreCase))
                        {
                            GlobalVars._Dialogue = false;
                            return "AI Dialogue checkbox will be stopped.";
                        }
                    }

                    // Respond to questions about the AI's identity.
                    if (result.Text.Contains("Who", StringComparison.OrdinalIgnoreCase) &&
                        (result.Text.Contains("are", StringComparison.OrdinalIgnoreCase) ||
                         result.Text.Contains("you", StringComparison.OrdinalIgnoreCase)))
                    {
                        return "This is the new version of 'Hello World'.";
                    }

                    // Default response if no specific command matches.
                    return "Command not found.";
                }
            }
            else if (result.Reason == ResultReason.NoMatch) // No speech was recognized.
            {
                return "Geen spraak herkend."; // Dutch: "No speech recognized."
            }
            else if (result.Reason == ResultReason.Canceled) // Recognition was canceled.
            {
                var cancellation = CancellationDetails.FromResult(result);

                if (cancellation.Reason == CancellationReason.Error)
                {
                    return "Foutdetails:" + cancellation.ErrorDetails; // Dutch: "Error details."
                }

                return "Geannuleerd:" + cancellation.Reason; // Dutch: "Canceled."
            }

            // Default response if no audio analysis occurred.
            return "None audio analysis happened";
        }
    }
}
