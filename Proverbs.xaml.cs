using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AVFoundation;

namespace ZoharBible;

/// <summary>
/// The Proverbs class represents a content page within the Zohar Bible application.
/// It is responsible for initializing the UI components and configuring the
/// application based on the user's AI selection for analyzing proverbs.
/// </summary>
public partial class Proverbs : ContentPage
{
    /// <summary>
    /// A static string variable that holds the translated text of a proverb or psalm.
    /// This variable is updated after retrieving and translating a proverb or psalm from a remote service.
    /// </summary>
    public static string translatedText = "";

    /// <summary>
    /// The Proverbs class represents a content page within the Zohar Bible application.
    /// It is responsible for initializing the UI components and configuring the
    /// application based on the user's AI selection for analyzing proverbs.
    /// </summary>
    public Proverbs()
    {
        InitializeComponent();
        try
        {
            ConfigureAudioSession();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"There was an error configuring the audio session: {ex.Message}", "OK");
        }

        try
        {
            UpdateCheckBoxes(GlobalVars.AiSelected);

            UpdateLabel("...");
            PartCheckBox.IsChecked = true;
            OnOptionButtonClicked(KabbalahButton, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"There was an error during initialization: {ex.Message}", "OK");
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GlobalVars._IntroPage = false;
    }
    #region Button Event Handlers

    /// <summary>
    /// Handles the text changed event for ProverbEditor.
    /// This method enables or disables buttons based on the length of the text in the editor.
    /// </summary>
    /// <param name="sender">The source of the event, typically the Editor whose text was changed.</param>
    /// <param name="e">EventArgs object containing the event data, including the new and old text values.</param>
    private void OnProverbEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var editor = sender as Editor;
            if (editor == null) return;
            var lineCount = editor.Text.Length;
            GetAnalysisButton.IsEnabled = lineCount > 100;
            StartSpeak.IsEnabled = lineCount > 100;
            StopSpeak.IsEnabled = lineCount > 100;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"There was an error processing text changes: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the click event of the "Get Zohar Explanation" button.
    /// When clicked, it displays a message indicating that a proverb is being retrieved,
    /// waits for 2 seconds, and then calls a REST API to get and display a random proverb from the Bible.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private async void OnGetZoharExplanationButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            GlobalVars._pPortion = "Proverbs";
            this.MessageLabel.IsVisible = true;
            UpdateLabel("Getting Verse");
            await Task.Delay(1000);
            string responseText = "";
            this.ProverbEditor.Text = "";
            if (this.ProverbNumberEntry.Text.Equals("0"))
            {
                if (PartCheckBox.IsChecked)
                {
                    if (GlobalVars._ProverbOrPsalm.Contains("Proverbs"))
                        responseText =
                            await GlobalVars.GetHttpReturnFromAPIRestLink(
                                Secrets.RESTAPI + @"BibleProverbsPart");
                    else
                        responseText =
                            await GlobalVars.GetHttpReturnFromAPIRestLink(
                                Secrets.RESTAPI + @"BiblePsalmsPart");
                    responseText = AddCommaToPsalmOrProverbName(responseText);
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                }
                else
                {
                    if (GlobalVars._ProverbOrPsalm.Contains("Proverbs"))
                        responseText =
                            await GlobalVars.GetHttpReturnFromAPIRestLink(
                                Secrets.RESTAPI + @"BibleProverbs");
                    else
                        responseText =
                            await GlobalVars.GetHttpReturnFromAPIRestLink(
                                Secrets.RESTAPI + @"BiblePsalms");
                    responseText = AddCommaToPsalmOrProverbName(responseText);
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                }
            }
            else
            {
                if (PartCheckBox.IsChecked)
                {
                    if (GlobalVars._ProverbOrPsalm.Contains("Proverbs"))
                        responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                            Secrets.RESTAPI + @"BibleProverbsPart/EN" +
                            this.ProverbNumberEntry.Text);
                    else
                        responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                            Secrets.RESTAPI + @"BiblePsalmsPart/EN" +
                            this.ProverbNumberEntry.Text);
                    responseText = AddCommaToPsalmOrProverbName(responseText);
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                }
                else
                {
                    if (GlobalVars._ProverbOrPsalm.Contains("Proverbs"))
                        responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                            Secrets.RESTAPI + @"BibleProverbs/EN" +
                            this.ProverbNumberEntry.Text);
                    else
                        responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                            Secrets.RESTAPI + @"BiblePsalms/EN" +
                            this.ProverbNumberEntry.Text);
                    responseText = AddCommaToPsalmOrProverbName(responseText);
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                }
            }

            this.ProverbEditor.Text = translatedText;
            GlobalVars.ProverbToAnalyse = ExtractProverbsAndNumber(responseText); // Save proverb number to analyse
            UpdateLabel("...");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an error getting Zohar explanation: {ex.Message}", "OK");
            UpdateLabel("Error retrieving verse");
        }
    }

    /// <summary>
    /// Handles the click event of the option buttons (Kabbalah, Zohar, Mishna, All).
    /// It updates the type of proverb analysis based on the text of the clicked button
    /// and disables all other option buttons except for the Reset button.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private async void OnOptionButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var clickedButton = sender as Button;
            GlobalVars.TypeOfProverbAnalysis = clickedButton.Text; // Save the type of analysis for Proverbs
            if (GlobalVars.TypeOfProverbAnalysis.Contains("All"))
                GlobalVars.TypeOfProverbAnalysis = " Kabbalah and the Zohar and the Mishna";
            foreach (var child in (clickedButton.Parent as StackLayout).Children)
            {
                if (child is Button button && button != clickedButton && button != ResetButton)
                {
                    button.IsEnabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event of the "Reset" button.
    /// When clicked, it re-enables all the primary buttons (Kabbalah, Zohar, Mishna, All)
    /// that allow the user to choose different sections of content within the Zohar Bible application.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private async void OnResetButtonClicked(object sender, EventArgs e)
    {
        try
        {
            KabbalahButton.IsEnabled = true;
            ZoharButton.IsEnabled = true;
            MishnaButton.IsEnabled = true;
            AllButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event of the "Get Analysis" button.
    /// Changes the button's color temporarily, displays a "Preparing Analysis"
    /// message, waits for 1 second, and navigates to the ChatAnalysis page.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private async void OnGetAnalysisButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);

            this.MessageLabel.IsVisible = true;
            UpdateLabel("Preparing Analysis");
            await Task.Delay(1000);
            await Navigation.PushAsync(new ChatAnalysis());
            UpdateLabel("...");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event of the "Start Speak" button.
    /// Changes the button color temporarily and initiates text-to-speech conversion of the translated text.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private async void OnStartSpeakClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            await GlobalVars.ttsService.ConvertTextToSpeechAsync(translatedText);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event of the "Stop Speak" button.
    /// When clicked, it temporarily changes the color of the button and stops any currently speaking text using the TextToSpeechService.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private async void OnStopSpeakClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);

            await GlobalVars.ttsService.StopSpeakingAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

   
    /// <summary>
    /// Handles the text changed event of the ProverbNumberEntry.
    /// Validates the entered text to ensure it is a valid number between 0 and 31.
    /// If the text is not a valid number, it sets the entry text to "0".
    /// If the number is greater than 31, it sets the entry text to "31".
    /// </summary>
    /// <param name="sender">The source of the event, typically the ProverbNumberEntry control.</param>
    /// <param name="e">A TextChangedEventArgs object that contains the event data.</param>
    private async void OnProverbNumberEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (int.TryParse(e.NewTextValue, out int number))
            {
                if (number > 31)
                {
                    ProverbNumberEntry.Text = "31";
                }
            }
            else
            {
                // Optioneel: Handelen in geval van ongeldige invoer, bijvoorbeeld:
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    ProverbNumberEntry.Text = "0";
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the CheckedChanged event for checkboxes within the Proverbs page.
    /// Ensures that only one of either the PartCheckBox or the FullCheckBox can be selected at a time.
    /// </summary>
    /// <param name="sender">The source of the event, typically the checkbox that was changed.</param>
    /// <param name="e">A CheckedChangedEventArgs object that contains the event data.</param>
    private async void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            if (sender == PartCheckBox && PartCheckBox.IsChecked)
            {
                FullCheckBox.IsChecked = false;
            }
            else if (sender == FullCheckBox && FullCheckBox.IsChecked)
            {
                PartCheckBox.IsChecked = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the event when the state of any of the chatbot selection checkboxes is changed.
    /// Ensures that only one chatbot checkbox can be checked at a time and updates the
    /// global variable to reflect the selected chatbot.
    /// </summary>
    /// <param name="sender">The source of the event, typically the checkbox that triggered the event.</param>
    /// <param name="e">The event data containing information about the checkbox state change.</param>
    private async void OnChatbotCheckBoxChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            if (sender == ChatGPTCheckBox && ChatGPTCheckBox.IsChecked)
            {
                GroKCheckBox.IsChecked = false;
                GeminiCheckBox.IsChecked = false;
                AllAICheckBox.IsChecked = false;
                GlobalVars.AiSelected = "ChatGPT";
            }
            else if (sender == GroKCheckBox && GroKCheckBox.IsChecked)
            {
                ChatGPTCheckBox.IsChecked = false;
                GeminiCheckBox.IsChecked = false;
                AllAICheckBox.IsChecked = false;
                GlobalVars.AiSelected = "GroK";
            }
            else if (sender == GeminiCheckBox && GeminiCheckBox.IsChecked)
            {
                ChatGPTCheckBox.IsChecked = false;
                GroKCheckBox.IsChecked = false;
                AllAICheckBox.IsChecked = false;
                GlobalVars.AiSelected = "Gemini";
            }
            else if (sender == AllAICheckBox && AllAICheckBox.IsChecked)
            {
                ChatGPTCheckBox.IsChecked = false;
                GroKCheckBox.IsChecked = false;
                GeminiCheckBox.IsChecked = false;
                GlobalVars.AiSelected = "AllAI";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}");
        }
    }

    #endregion
    
    #region Helper Methods

    /// <summary>
    /// Updates the state of various AI-related checkboxes based on the selected AI options.
    /// </summary>
    /// <param name="aiSelected">A string representing the selected AI options which may include "ChatGPT", "GroK", "Gemini", or "AllAI".</param>
    private void UpdateCheckBoxes(string aiSelected)
    {
        ChatGPTCheckBox.IsChecked = aiSelected.Contains("ChatGPT");
        GroKCheckBox.IsChecked = aiSelected.Contains("GroK");
        GeminiCheckBox.IsChecked = aiSelected.Contains("Gemini");
        AllAICheckBox.IsChecked = aiSelected.Contains("AllAI");

        if (!aiSelected.Contains("AllAI"))
        {
            AllAICheckBox.IsChecked = false;
        }
    }

    /// <summary>
    /// Displays an alert dialog with the specified title and message.
    /// </summary>
    /// <param name="title">The title of the alert dialog.</param>
    /// <param name="message">The message content of the alert dialog.</param>
    /// <returns>A task representing the asynchronous operation of displaying the alert.</returns>
    private async Task DisplayAlertAsync(string title, string message)
    {
        await DisplayAlert(title, message, "OK");
    }

    /// <summary>
    /// Adds a comma after the name of a Psalm or Proverb followed by a number in a given string.
    /// </summary>
    /// <param name="toAnalyze">The string to analyze and modify.</param>
    /// <returns>A new string where commas are added after "Proverbs" or "Psalms" names followed by numbers.</returns>
    private static string AddCommaToPsalmOrProverbName(string toAnalyze)
    {
        try
        {
            if (toAnalyze.Contains("Proverbs"))
            {
                string pattern = @"(Proverbs\s*\d+)";
                string result = Regex.Replace(toAnalyze, pattern, m => $"{m.Value},");
                return result;
            }
            else
            {
                string pattern = @"(Psalms\s*\d+)";
                string result = Regex.Replace(toAnalyze, pattern, m => $"{m.Value},");
                return result;
            }
        }
        catch (Exception ex)
        {
            toAnalyze = "Error: " + ex.Message;
            return toAnalyze;
        }
    }

    /// <summary>
    /// Extracts the book name (either "Proverbs" or "Psalms") and the number from the given input string.
    /// If the input contains the specified book name followed by a space and a number, it returns the matched string.
    /// Otherwise, it returns "No match found".
    /// </summary>
    /// <param name="input">The input string to search for the book name and number.</param>
    /// <returns>A string containing the matched book name and number or "No match found".</returns>
    static string ExtractProverbsAndNumber(string input)
    {
        try
        {
            string ww = GlobalVars._ProverbOrPsalm.Contains("Proverbs") ? "Proverbs" : "Psalms";
            // Regular expression to match "Proverbs" followed by a space and a number
            Regex regex = new Regex(@"(" + ww + @")\s(\d+)");
            var match = regex.Match(input);
            return match.Success ? match.Value : "No match found";
        }
        catch (Exception ex)
        {
            return "No match found:" + ex.Message;
        }
    }

    /// <summary>
    /// Updates the text of the MessageLabel on the UI thread and ensures the label is visible.
    /// </summary>
    /// <param name="text">The new text to be updated in the MessageLabel.</param>
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
            await DisplayAlertAsync("Error", $"An error occurred while updating the label: {ex.Message}");
        }
    }

    /// <summary>
    /// Configures the audio session for playback using AVAudioSession.
    /// Sets the category to AVAudioSessionCategory.Playback and activates the session.
    /// </summary>
    private void ConfigureAudioSession()
    {
        var audioSession = AVAudioSession.SharedInstance();
        audioSession.SetCategory(AVAudioSessionCategory.Playback);
        //audioSession.SetMode(AVAudioSessionMode.SpokenAudio);
        audioSession.SetActive(true);
    }
    #endregion
}