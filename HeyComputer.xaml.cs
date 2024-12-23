using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Newtonsoft.Json.Linq;
using PassKit;
using RestSharp;

namespace ZoharBible
{
    /// <summary>
    /// Represents the main page for the 'HeyComputer' functionality, handling UI interactions like button clicks, audio recording, and video playback.
    /// </summary>
    public partial class HeyComputer : ContentPage
    {
        /// <summary>
        /// The audio service used for recording and managing audio input.
        /// </summary>
        private readonly IAudioService _audioService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeyComputer"/> class, setting up UI components and audio services.
        /// </summary>
        public HeyComputer()
        {
            InitializeComponent();
            
            // Set the width of the generated image and video player to half the screen width
            GeneratedImage.WidthRequest = DeviceDisplay.MainDisplayInfo.Width / 2;
            VideoPlayer.WidthRequest = DeviceDisplay.MainDisplayInfo.Width / 2;
            
            // Initialize and set the view model for data binding
            var viewModel = new MainViewModelChatGPT();
            this.BindingContext = viewModel;
            
            // Initialize the audio service
            IAudioService audioService = new AudioService();
            _audioService = audioService;
            
            // Increase microphone sensitivity for better audio capture
            _audioService.IncreaseMicrophoneSensitivity();
            
            // Trigger the button click event to start with the initial state
            object sender = this.BlinkingButton;
            EventArgs e = new EventArgs();
            OnButtonEntryClicked(sender, e);
        }

        /// <summary>
        /// Handles the toggle for starting or stopping the talk mode, which changes the button's appearance and state.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnButtonEntryClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            if (GlobalVars.HeyComputerStartTalk)
            {
                // Stop talking mode
                GlobalVars.HeyComputerStartTalk = false;
                button.BackgroundColor = Color.FromArgb("#00FF00"); 
                button.TextColor = Color.FromArgb("#00008B"); 
                button.BorderColor = Color.FromArgb("#FFFF00");
                button.Text = "Start Talking";
                StatusLabel.Text = "Press button to start recording.";
                
                // Animate button border color to indicate the mode
                while (true)
                {
                    await button.AnimateBorderColor(Color.FromArgb("#00FFFFFF"), 500);
                    await button.AnimateBorderColor(Color.FromArgb("#FFFF00"), 500); // Animate to yellow
                }
            }
            else
            {
                // Start talking mode
                GlobalVars.HeyComputerStartTalk = true;
                button.BackgroundColor = Color.FromArgb("#FF0000");
                button.TextColor = Color.FromArgb("#FFA500");
                button.Text = "Stop Talking";
                StatusLabel.Text = "Press button to stop recording";
                
                // Animate button border color to indicate the mode
                while (true)
                {
                    await button.AnimateBorderColor(Color.FromArgb("#00FFFFFF"), 500);
                    await button.AnimateBorderColor(Color.FromArgb("#FFA500"), 500);
                }
            }
        }

        /// <summary>
        /// Manages the action when the record button is clicked, controlling audio recording and transcription.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnButtonClicked(object sender, EventArgs e)
        {
            this.VideoPlayer.IsVisible = false;
            var button = sender as Button;
            if (button == null)
                return;

            if (GlobalVars.HeyComputerStartTalk)
            {
                // Stop recording
                _audioService.StopRecording();
                GlobalVars.HeyComputerStartTalk = false;
                button.BackgroundColor = Color.FromArgb("#00FF00"); 
                button.TextColor = Color.FromArgb("#00008B"); 
                button.BorderColor = Color.FromArgb("#FFFF00");
                button.Text = "Start Talking";
                StatusLabel.Text = "Press button to start recording.";
                VoiceLabel.Text = "Transcribing";

                // Path for the recorded audio file
                string _filePathIn = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "recording.wav");

                // Keywords for speech recognition
                List<string> keywords = new List<string>
                {
                    "X", "Google", "Microsoft", "Elon", "Gemini", "Bill"
                };

                // Transcribe audio and update UI with results
                string returN = await AzureSpeechToText.TranscribeAudioAsync(_filePathIn, keywords);
                VoiceLabel.Text = returN;
                SetImageSource(returN);

                // Animate button border color to indicate the mode
                while (true)
                {
                    await button.AnimateBorderColor(Color.FromArgb("#00FFFFFF"), 500);
                    await button.AnimateBorderColor(Color.FromArgb("#FFFF00"), 500); // Animate to yellow
                }
            }
            else
            {
                // Start recording
                GlobalVars.HeyComputerStartTalk = true;
                button.BackgroundColor = Color.FromArgb("#FF0000");
                button.TextColor = Color.FromArgb("#FFA500");
                button.Text = "Stop Talking";
                _audioService.StartRecording();
                StatusLabel.Text = "Press button to stop recording";

                // Animate button border color to indicate the mode
                while (true)
                {
                    await button.AnimateBorderColor(Color.FromArgb("#00FFFFFF"), 500);
                    await button.AnimateBorderColor(Color.FromArgb("#FFA500"), 500);
                }
            }
        }

        /// <summary>
        /// Sets the image source based on the transcribed text, potentially triggering video creation for specific keywords.
        /// </summary>
        /// <param name="imageReturn">The text returned from speech recognition which might contain keywords for image generation.</param>
        private async void SetImageSource(string imageReturn)
        {
            if (imageReturn.Contains("Talked Bill"))
            {
                GlobalVars.DallE_Image_string = "Create an image of Bill Gates as the weird Einstein";
                GlobalVars.videoTalked = "Bill";
                if (GlobalVars.AIInteractive)
                {
                    if (this.CheckboxHeyGen.IsChecked ||
                        this.CheckboxHeyGenStandard.IsChecked)
                    {
                        string altText = "";
                        await DoTheTalkingTom(altText);    
                    }
                    if (this.CheckboxDID.IsChecked)
                    {
                        await Do_DIDTalk();
                    }
                }
                else
                {
                    GlobalVars.videoTalkedText = "This is the return from Bill Gates";
                    await CreateVideoClipClicked();
                }


            }

            if (imageReturn.Contains("Talked Elon"))
            {
                GlobalVars.DallE_Image_string = "Create an image of Elon Musk as the weird Einstein";
                GlobalVars.videoTalked = "Elon";
                if (GlobalVars.AIInteractive)
                {
                    if (this.CheckboxHeyGen.IsChecked ||
                        this.CheckboxHeyGenStandard.IsChecked)
                    {
                        string altText = "";
                        await DoTheTalkingTom(altText);    
                    }
                    if (this.CheckboxDID.IsChecked)
                    {
                        await Do_DIDTalk();
                    }
                }
                else
                {
                    GlobalVars.videoTalkedText = "This is the return from Elon Musk";
                    await CreateVideoClipClicked();
                }
            }

            if (imageReturn.Contains("Talked Gemini"))
            {
                GlobalVars.DallE_Image_string = "Create an image of the crazy Einstein";
                GlobalVars.videoTalked = "Google";
                if (GlobalVars.AIInteractive)
                {
                    if (this.CheckboxHeyGen.IsChecked ||
                        this.CheckboxHeyGenStandard.IsChecked)
                    {
                        string altText = "";
                        await DoTheTalkingTom(altText);    
                    }
                    if (this.CheckboxDID.IsChecked)
                    {
                        await Do_DIDTalk();
                    }
                }
                else
                {
                    GlobalVars.videoTalkedText = "This is the return from Google";
                    await CreateVideoClipClicked();
                }
            }
        }

        private async Task Do_DIDTalk()
        {
            GlobalVars.videoTalked = "InteractiveStart";
            GlobalVars.anim = true;
            await CreateVideoClipClicked();
            GlobalVars.anim = false;
            GlobalVars.videoTalkedText = GlobalVars.AIInteractiveText;
            GlobalVars.videoTalked = "Interactive";
            await CreateVideoClipClicked();
        }

        private async Task DoTheTalkingTom(string alternateText)
        {
            GlobalVars.videoTalked = "InteractiveStart";
            GlobalVars.anim = true;
            await CreateVideoClipClicked();
            GlobalVars.anim = false;

            if (alternateText.Length >= 2)
            {
                GlobalVars.videoTalkedText = alternateText;
            }
            else
            {
                GlobalVars.videoTalkedText = GlobalVars.AIInteractiveText;
            }
            GlobalVars.videoTalked = "Interactive";
            VideoPlayer.Source = await DoVideoClipLipSync.PlayExistingVideo("loading.mp4");
            await OnHClicked(GlobalVars.videoTalkedText);
            VideoPlayer.Source = await DoVideoClipLipSync.PlayExistingVideo("loading.mp4");
            await OnVClicked();
        }
        private void OnHeyGenCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (CheckboxHeyGen.IsChecked)
            {
                CheckboxHeyGenStandard.IsChecked = false;
                CheckboxDID.IsChecked = false;
            }
        }

        private void OnHeyGenStandardCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (CheckboxHeyGenStandard.IsChecked)
            {
                CheckboxHeyGen.IsChecked = false;
                CheckboxDID.IsChecked = false;
            }
        }

        private void OnDIDCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (CheckboxDID.IsChecked)
            {
                CheckboxHeyGen.IsChecked = false;
                CheckboxHeyGenStandard.IsChecked = false;
            }
        }
        #region Audio

        /// <summary>
        /// Stops the ongoing speech synthesis if any.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnStopSpeakClicked(object sender, EventArgs e)
        {
            try
            {
                await GlobalVars.ttsService.StopSpeakingAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        // Eventhandler voor CheckboxTomMiddle
        private void OnTomMiddleCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // Controleer of de checkbox is ingeschakeld
            if (e.Value)
            {
                this.CheckboxTomYoung.IsChecked = false;
            }
            else
            {
               
            }
        }

        // Eventhandler voor CheckboxTomYoung
        private void OnTomYoungCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // Controleer of de checkbox is ingeschakeld
            if (e.Value)
            {
                this.CheckboxTomMiddle.IsChecked = false;
            }
            else
            {
               
            }
        }
        /// <summary>
        /// Toggles the AI interaction mode which affects how video content is generated.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnAIInteractiveChanged(object? sender, EventArgs e)
        {
            if (GlobalVars.AIInteractive)
            {
                GlobalVars.AIInteractive = false;
                this.CheckboxAIInteractive.IsChecked = false;
            }
            else
            {
                GlobalVars.AIInteractive = true;
                this.CheckboxAIInteractive.IsChecked = true;
            }
        }

        private string _vvvid = "";
        private async Task OnHClicked(string AItext)
        {
            // Middle tom bc839a7a94e941fd8ddf2d209ae4a7be
            // Young tom b77aad01446c4cfd97ee6a2691534687
            string __photoID = "";
            
            if (CheckboxTomYoung.IsChecked)
                __photoID = "b77aad01446c4cfd97ee6a2691534687";
            if (CheckboxTomMiddle.IsChecked)
                __photoID = "bc839a7a94e941fd8ddf2d209ae4a7be";
            string _id = "";
            
            if(this.CheckboxHeyGen.IsChecked) // Own photo avatar
                _id = await DoVideoClipLipSync.CreateHeyGenTalkingPhotoVideo(Secrets.hgenKey, AItext, __photoID);
            else // Standard video avatar
                _id = await DoVideoClipLipSync.CreateHeyGenVideoAvatar(Secrets.hgenKey, AItext);
            
            _vvvid = await DoVideoClipLipSync.getStringFromJson(_id);
            //await DisplayAlert("Message", "Ran the Video Creation", "OK");

        }
        private async Task OnVClicked()
        {
            string _vid = "null";
            while (_vid.Equals("null") || _vid.Length == 0)
            {
                string _vidd = await DoVideoClipLipSync.checkVideoID(
                    Secrets.hgenKey,
                    _vvvid);
                _vid = GetVideoUrlFromJson(_vidd);
                VideoPlayer.Source = await DoVideoClipLipSync.PlayExistingVideo("loading.mp4");
            }
            // await DisplayAlert("Message", "We got the utl:" + '\n' + '\n' +
            //     _vid, "OK");
            VideoPlayer.IsVisible = true;
            VideoPlayer.Source = _vid;
            VideoPlayer.Play();
        }
        public static string GetVideoUrlFromJson(string jsonString)
        {
            try
            {
                // Parse the JSON string
                JObject json = JObject.Parse(jsonString);

                // Navigate to "data" -> "video_url"
                string videoUrl = json["data"]?["video_url"]?.ToString();

                // Return the video_url value or a default message if not found
                return videoUrl ?? "video_url not found";
            }
            catch (Exception ex)
            {
                return $"Error parsing JSON: {ex.Message}";
            }
        }

        private async void OnCreditsClicked(object? sender, EventArgs e)
        {
            string gC = await DoVideoClipLipSync.GetCredits();
            var doc = JsonDocument.Parse(gC);
            var creditElement = doc.RootElement.GetProperty("credits")[0];

            var credit = new Credit
            {
                Total = creditElement.GetProperty("total").GetInt32(),
                Remaining = creditElement.GetProperty("remaining").GetInt32(),
                CreatedAt = creditElement.GetProperty("created_at").GetDateTime(),
                ExpireAt = creditElement.GetProperty("expire_at").GetDateTime(),
                ProductBillingInterval = creditElement.GetProperty("product_billing_interval").GetString()
            };
            await DisplayAlert("Credit Info",
                "Total Credits:" + credit.Total.ToString() + '\n' +
                "Remaining Credits:" + credit.Remaining.ToString() + '\n' +
                "Created at:" + credit.CreatedAt + '\n' +
                "TExpire at:" + credit.ExpireAt + '\n' +
                "Interval:" + credit.ProductBillingInterval + '\n', "OK");
        }
        private async void OnCreditsHClicked(object? sender, EventArgs e)
        {
            var options = new RestClientOptions("https://api.heygen.com/v2/user/remaining_quota");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("x-api-key", "ZWYwZjgyN2NhMWFiNGIzNGI5NTYxNjg4NDA2YjcxYWItMTczNDE5OTM3MA==");
            var response = await client.GetAsync(request);
            using JsonDocument doc = JsonDocument.Parse(response.Content);

            // Navigate to "remaining_quota"
            int remainingQuota = doc.RootElement
                .GetProperty("data")
                .GetProperty("remaining_quota")
                .GetInt32();
            double _remainingQ = remainingQuota / 60;
            await DisplayAlert("Credit Info",
                "Remaining Credits:" + _remainingQ.ToString()
                , "OK");
        }
        public class Credit
        {
            public int Total { get; set; }
            public int Remaining { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpireAt { get; set; }
            public string ProductBillingInterval { get; set; }
        }
        /// <summary>
        /// Initiates the creation of a video clip with lip-sync based on the current text input.
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnCreateVideoClipClicked(object? sender, EventArgs e)
        {
            await CreateVideoClipClicked();
        }
        private async Task CreateVideoClipClicked()
        {
            try
            {
                this.VideoPlayer.IsVisible = true;

                string theVideo = "";
                if (GlobalVars.anim)
                    theVideo = await DoVideoClipLipSync.CreateAnimationAvatar(GlobalVars.videoTalkedText);
                else
                    theVideo = await DoVideoClipLipSync.CreateClipAvatar(GlobalVars.videoTalkedText);

                if (GlobalVars.videoFileExists)
                {
                    if(GlobalVars.videoTalked.Contains("InteractiveStart"))
                        VideoPlayer.Source = await DoVideoClipLipSync.PlayExistingVideo("loading.mp4");
                    else if (GlobalVars.videoTalked.Contains("Interactive"))
                    {
                        theVideo = await DoVideoClipLipSync.CreateClipAvatar(GlobalVars.videoTalkedText);
                        string urll = await DoVideoClipLipSync.GetVideoClip(theVideo);
                        await LoadAndPlayVideo(urll);
                    }
                    else
                        await PlayVideosInSequenceAsync();
                }
                else
                {
                    string urll = await DoVideoClipLipSync.GetVideoClip(theVideo);
                    await LoadAndPlayVideo(urll);    
                }
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }


        /// <summary>
        /// Plays two videos in sequence, used when a video file already exists.
        /// </summary>
        private async Task PlayVideosInSequenceAsync()
        {
            VideoPlayer.MediaEnded += OnFirstVideoEnded;

            // Start the first video
            VideoPlayer.Source = await DoVideoClipLipSync.PlayExistingVideo(GlobalVars.videoFilePath);
            GlobalVars.videoFileExists = false;
            GlobalVars.videoFilePath = "";
            GlobalVars.anim = true;
        }

        /// <summary>
        /// Event handler for when the first video ends, starts playing the second video.
        /// </summary>
        private async void OnFirstVideoEnded(object sender, EventArgs e)
        {
            VideoPlayer.MediaEnded -= OnFirstVideoEnded;
            await DoVideoClipLipSync.CreateAnimationAvatar(GlobalVars.videoTalkedText);
            GlobalVars.videoFilePath = "TalkedAnimation.mp4";
            // Start the second video
            var localFilePath = await DoVideoClipLipSync.PlayExistingVideo(GlobalVars.videoFilePath);
            VideoPlayer.Source = localFilePath;
            GlobalVars.videoFileExists = false;
            GlobalVars.videoFilePath = "";
            GlobalVars.anim = false;
        }

        /// <summary>
        /// Updates the message label with text on the main thread to ensure UI thread safety.
        /// </summary>
        /// <param name="text">The text to be displayed in the message label.</param>
        private async void UpdateLabel(string text)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.MessageLabel.IsVisible = true;
                    this.MessageLabel.Text = text;
                });
                await Task.Yield();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Message:" + ex.Message, "OK");
            }
        }

        /// <summary>
        /// Downloads a video from a URL and sets it as the source for the video player.
        /// </summary>
        /// <param name="vidUrl">The URL of the video to download and play.</param>
        private async Task LoadAndPlayVideo(string vidUrl)
        {
            try
            {
                string localFilePath = await DoVideoClipLipSync.DownloadVideoAsync(vidUrl);
                VideoPlayer.Source = localFilePath;
 
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load video: {ex.Message}", "OK");
            }
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for animations, particularly for UI elements like buttons.
    /// </summary>
    public static class AnimationExtensions
    {
        /// <summary>
        /// Animates the border color of a button over a specified duration.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <param name="targetColor">The color to animate towards.</param>
        /// <param name="duration">The duration of the animation in milliseconds.</param>
        public static async Task AnimateBorderColor(this Button button, Color targetColor, uint duration)
        {
            var animation = new Animation(v =>
            {
                button.BorderColor = Color.FromRgba(v, button.BorderColor.Green, button.BorderColor.Blue, button.BorderColor.Alpha);
            }, button.BorderColor.Red, targetColor.Red);

            animation.Commit(button, "BorderColorAnimation", 16, duration, Easing.Linear, (v, c) => button.BorderColor = targetColor);
            await Task.Delay((int)duration);
        }
    }
}