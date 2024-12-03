using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoharBible;

/// <summary>
/// Represents a page for testing audio recording functionality within the application.
/// </summary>
public partial class TestAudioRecording : ContentPage
{
    /// <summary>
    /// The audio service used for recording, playing, and managing audio.
    /// </summary>
    private readonly IAudioService _audioService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAudioRecording"/> class.
    /// </summary>
    public TestAudioRecording()
    {
        InitializeComponent();
        IAudioService audioService = new AudioService();
        _audioService = audioService;
        // Increases microphone sensitivity for better audio capture
        _audioService.IncreaseMicrophoneSensitivity();
    }

    #region Recording

    /// <summary>
    /// Handles the click event of the record button to start audio recording.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnRecordButtonClicked(object sender, EventArgs e)
    {
        try
        {
            _audioService.StartRecording();
            StatusLabel.Text = "Recording...";
            await Task.Delay(1000);
            StatusLabel.Text = "...";
            RecordButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            PlayButton.IsEnabled = false;
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Handles the click event of the stop button to stop audio recording.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnStopButtonClicked(object sender, EventArgs e)
    {
        try
        {
            _audioService.StopRecording();
            StatusLabel.Text = "Recording stopped.";
            await Task.Delay(1000);
            StatusLabel.Text = "...";
            RecordButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            PlayButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Handles the click event of the play button to play the recorded audio.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnPlayButtonClicked(object sender, EventArgs e)
    {
        try
        {
            _audioService.PlayRecording();
            StatusLabel.Text = "Playing recording...";
            await Task.Delay(1000);
            StatusLabel.Text = "...";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Handles the click event for transcribing recorded audio, checking for specific keywords and responding accordingly.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnTranscribeClicked(object sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "Transcribing...";
            await Task.Delay(1000);
            // Path to the recorded audio file
            string _filePathIn = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "recording.wav");
            List<string> keywords = new List<string>
            {
                "Dialogue", "dialogue", "Dialog", "dialog", "Who", "who"
            };
            string returN = await AzureSpeechToText.TranscribeAudioAsync(_filePathIn, keywords);

            // Check for specific phrases in the transcription
            if (returN.Contains("Dialogue", StringComparison.OrdinalIgnoreCase) 
                && returN.Contains("Started", StringComparison.OrdinalIgnoreCase))
            {
                VoiceLabel.Text = returN;
                string dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/"
                                    + "Tell the user you will start the checkbox called AI dialogue" +
                                    " to enable a dialogue between the AI providers.");
                await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);
            }
            else if (returN.Contains("Dialogue", StringComparison.OrdinalIgnoreCase) 
                && returN.Contains("Stopped", StringComparison.OrdinalIgnoreCase))
            {
                VoiceLabel.Text = returN;
                string dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/"
                                    + "Tell the user you will stop the checkbox called AI dialogue" +
                                    " to disable the dialogue between the AI providers.");
                await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);
            }
            else if (returN.Contains("Hello", StringComparison.OrdinalIgnoreCase) 
                && returN.Contains("World", StringComparison.OrdinalIgnoreCase))
            {
                VoiceLabel.Text = returN;
                string dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/"
                                    + "Tell the user that this is a new era of the old example of the 'Hello World'" +
                                    " example program that was always used when new programming languages were introduced." +
                                    " Now this current version of the 'Hello Work' is the door to a new world, a world " +
                                    " where AI is your companion.");
                await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);
            }
            else if (returN.Contains("Command not found"))
            {
                VoiceLabel.Text = returN;
                string dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/"
                                    + "Tell the user you did not find the command valid " +
                                    " and you cannot help the user.");
                await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);
            }

            VoiceLabel.Text = "...";
            StatusLabel.Text = "...";
        }
        catch (Exception ex)
        {
            VoiceLabel.Text = $"Error: {ex.Message}";
        }
    }

    #endregion
}