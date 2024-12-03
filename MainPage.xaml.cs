using System.Runtime.InteropServices.JavaScript;

namespace ZoharBible;

/// <summary>
/// Represents the main page of the ZoharBible application.
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
        OnOptionButtonClicked(KabbalahButton, EventArgs.Empty);
        UpdateCheckBoxes(GlobalVars.AiSelected);
    }

    /// <summary>
    /// Called when the page becomes visible, updates global state for page navigation.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GlobalVars._IntroPage = false;
    }

    /// <summary>
    /// Updates the checkboxes based on the currently selected AI model.
    /// </summary>
    /// <param name="aiSelected">A string indicating which AI models are selected.</param>
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
    /// Handles the SelectedIndexChanged event of the LanguagePicker.
    /// Updates the current language in the GlobalVars class based on the selected item in the Picker.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the Picker that triggered the event.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnLanguagePickerChanged(object sender, EventArgs e)
    {
        try
        {
            var picker = sender as Picker;
            if (picker != null)
            {
                GlobalVars.lLanguage_ = picker.SelectedItem as string;
                // For debugging or further processing
                await DisplayAlert("Geselecteerde taal", GlobalVars.lLanguage_, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Get a random Proverb" button.
    /// Initializes some global variables related to the Proverbs page and
    /// navigates the user to the Proverbs page.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnGetProverbButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            UpdateLabel("Preparing the Screen...");
            await Task.Delay(1000);
            GlobalVars.Amida_ = "";
            GlobalVars._ProverbOrPsalm = "Proverbs";
            await Navigation.PushAsync(new Proverbs());
            UpdateLabel("...");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Get a random Psalm" button.
    /// Navigates the user to the Psalms page by updating relevant global variables.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnGetPsalmsButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            UpdateLabel("Preparing the Screen...");
            await Task.Delay(1000);
            GlobalVars.Amida_ = "";
            GlobalVars._ProverbOrPsalm = "Psalms";
            await Navigation.PushAsync(new Proverbs());
            UpdateLabel("...");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Get the Amida" button.
    /// Updates global state and navigates the user to the Chat Analysis page.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnGetAmidaButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            GlobalVars.Amida_ = "Amida";
            this.MessageLabel.IsVisible = true;
            UpdateLabel("Preparing Analysis");
            await Task.Delay(1000);
            await Navigation.PushAsync(new ChatAnalysis());
            UpdateLabel("...");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Get the Shema" button.
    /// Navigates the user to the ChatAnalysis page after a brief delay, updating the UI to prepare for the analysis.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnGetShemaButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            GlobalVars.Amida_ = "Shema";
            this.MessageLabel.IsVisible = true;
            UpdateLabel("Preparing Analysis");
            await Task.Delay(1000);
            await Navigation.PushAsync(new ChatAnalysis());
            UpdateLabel("...");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Updates the text of the MessageLabel on the main UI thread to the specified text.
    /// Ensures the MessageLabel is visible.
    /// </summary>
    /// <param name="text">The text to be displayed in the MessageLabel.</param>
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
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the CheckedChanged event of the chatbot selection checkboxes.
    /// Updates the selected chatbot in the global variables and ensures only one checkbox is selected at a time.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the checkbox that was interacted with.</param>
    /// <param name="e">A CheckedChangedEventArgs that contains the event data.</param>
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
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Shema Time" button.
    /// Fetches the time for the Shema prayer from an external API and displays it in an alert dialog.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnShemaTimeButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            string _shema = "SHEMA TIME: " + GlobalVars.GetHttpReturnFromAPIRestLink(
                Secrets.RESTAPI + @"Google/"
                + "Give ONLY the time the Shema should be prayed during the day");
            await DisplayAlert("Shema Time", _shema, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Amida Time" button.
    /// Retrieves and displays the time the Amida prayer should be performed.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnAmidaTimeButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            string _amida = "AMIDA TIME: " + GlobalVars.GetHttpReturnFromAPIRestLink(
                Secrets.RESTAPI + @"Google/"
                + "Give ONLY the time the Amida should be prayed during the day") + '\n';
            await DisplayAlert("Amida Time", _amida, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Analyze given text" button.
    /// Prepares and navigates the user to the ChatAnalysis page for further text analysis.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnGetAnalysisButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            GlobalVars.Amida_ = "AnalyzeDailyStudy";
            GlobalVars._pPortion = this.TopEntryBox.Text;
            GlobalVars.ProverbToAnalyse = this.TopEntryBox.Text;
            this.MessageLabel.IsVisible = true;
            UpdateLabel("Preparing Analysis of the given text");
            await Task.Delay(1000);
            await Navigation.PushAsync(new ChatAnalysis());
            UpdateLabel("...");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Focused event of the TopEntryBox.
    /// Clears the text in the entry box when it receives focus.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the entry box that received focus.</param>
    /// <param name="e">A FocusEventArgs that contains the event data.</param>
    private async void TopEntryBox_Focused(object sender, FocusEventArgs e)
    {
        try
        {
            TopEntryBox.Text = string.Empty; // Clear text when Entry gets focus
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the option buttons.
    /// Updates the type of proverb analysis based on the clicked button's text
    /// and disables other option buttons.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
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
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Reset" button.
    /// Resets the state of all option buttons to enabled.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the reset button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
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
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles the Click event of the "Get Daily Study" button.
    /// Navigates the user to the Chabat page.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnChabatButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            await Navigation.PushAsync(new ChabatPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }
}