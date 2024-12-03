using System.ComponentModel;
using System.Globalization;

namespace ZoharBible;

/// <summary>
/// The StartPage class is a part of the ZoharBible namespace and
/// inherits from ContentPage. It serves as the initial page for
/// the application's navigation and contains various UI elements
/// such as labels, sliders, and pickers.
/// </summary>
public partial class StartPage : ContentPage
{
    /// <summary>
    /// The <c>StartPage</c> class is the main entry point of the application, inheriting from <c>ContentPage</c>.
    /// It initializes necessary components and sets the initial state of the user interface.
    /// </summary>
    public StartPage()
    {
        InitializeComponent();
        
        try
        {
            if (this.BindingContext is ViewModelLanguageChanged viewModel)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }
        catch (Exception e)
        {
            DisplayAlert("Error", e.Message, "OK");
            throw;
        }
        
        
        GlobalVars.AiSelected = "ChatGPT";
        UpdateCheckBoxes(GlobalVars.AiSelected);
        SpeechSpeedSlider.Value = 90;
        
        // Stel de initiële waarde van de Label in
        SpeechSpeedValueLabel.Text = GlobalVars.SpeechSpeed.ToString() + "%";
    }
    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModelLanguageChanged.SelectedLanguage))
        {
            // Your logic when text changes
            var viewModel = (ViewModelLanguageChanged)sender;
            var newText = viewModel.SelectedLanguage;
            // Handle the text change here
            GlobalVars.LanguageChoosenByFullName = this.LanguageByName.Text;
        }
    }
    /// <summary>
    /// Navigates to the 'Hey Computer' page, setting up for interaction and updating the theme if necessary.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnNavigateToHeyComputerPageClicked(object sender, EventArgs e)
    {
        GlobalVars.HeyComputerStartTalk = true;
        GlobalVars.LanguageChoosenByFullName = this.LanguageByName.Text;
        await GlobalVars.SetClickedColor(sender);
        UpdateLabel("Opening 'Hey Computer' Page");
        await Task.Delay(1000);
        await Navigation.PushAsync(new HeyComputer());
    }
    #region Lifecycle Methods

    /// <summary>
    /// This method is called when the page is about to appear on the screen.
    /// It sets the selected language in the LanguagePicker to "en" (English) and
    /// initializes the MessageLabel text to "...".
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguagePicker.SelectedItem = "en";
        this.MessageLabel.Text = "...";
        GlobalVars._IntroPage = false;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the value change event for the speech speed slider.
    /// Updates the global speech speed setting and the corresponding label to display the new speed.
    /// </summary>
    /// <param name="sender">The source of the event. This is typically the slider.</param>
    /// <param name="e">The event arguments containing the old and new values of the slider.</param>
    private void OnSpeechSpeedSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        GlobalVars.SpeechSpeed = $"{e.NewValue}";
        SpeechSpeedValueLabel.Text = e.NewValue.ToString("F0") + "%";
    }

    /// <summary>
    /// Event handler for navigating to the main page when the corresponding button is clicked.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, the button that was clicked.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private async void OnNavigateToMainPageClicked(object sender, EventArgs e)
    {
        await GlobalVars.SetClickedColor(sender);
        UpdateLabel("Opening Starting Page");
        await Task.Delay(1000);
        await Navigation.PushAsync(new MainPage());
    }

    /// <summary>
    /// Handles the event when the "Start Horoscope" button is clicked.
    /// Updates the label with a message and navigates to the Horoscope page after a brief delay.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button.</param>
    /// <param name="e">The event data.</param>
    private async void OnNavigateToHoroPageClicked(object sender, EventArgs e)
    {
        await GlobalVars.SetClickedColor(sender);
        UpdateLabel("Opening Horoscope Page");
        await Task.Delay(1000);
        await Navigation.PushAsync(new Horoscope());
    }

    /// <summary>
    /// Event handler for the navigation button click event to navigate to the Tarot page.
    /// </summary>
    /// <param name="sender">The source of the event, typically a button.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnNavigateToTarotPageClicked(object sender, EventArgs e)
    {
        await GlobalVars.SetClickedColor(sender);
        UpdateLabel("Opening Tarot Page");
        await Task.Delay(1000);
        await Navigation.PushAsync(new Tarot());
    }

    /// <summary>
    /// Displays an alert message asynchronously.
    /// </summary>
    /// <param name="title">The title of the alert.</param>
    /// <param name="message">The message content of the alert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task DisplayAlertAsync(string title, string message)
    {
        await DisplayAlert(title, message, "OK");
    }

    /// <summary>
    /// Handles changes in the language picker selection.
    /// Updates the global language variable based on the selected language.
    /// </summary>
    /// <param name="sender">The object that triggered the event, expected to be a Picker.</param>
    /// <param name="e">Event arguments associated with the change in selection.</param>
    private void OnLanguagePickerChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        if (picker != null)
        {
            GlobalVars.lLanguage_ = picker.SelectedItem as string;
        }
        if (sender is Picker ppicker && ppicker.SelectedIndex != -1)
        {
            string selectedLanguage = ppicker.Items[ppicker.SelectedIndex];
            var viewModel = (ViewModelLanguageChanged)BindingContext;
            viewModel.SelectedLanguage = selectedLanguage;
        }
    }

    /// <summary>
    /// Event handler for changing the AI chatbot selection based on the checked checkboxes.
    /// Updates the checked state of other checkboxes to ensure only one is selected at a time
    /// and sets the selected AI in the global variables.
    /// </summary>
    /// <param name="sender">The checkbox that triggered the event.</param>
    /// <param name="e">Event arguments providing the new state of the checkbox.</param>
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
    #endregion

    #region Helper Methods

    /// <summary>
    /// Updates the state of the checkboxes based on the selected AI option.
    /// </summary>
    /// <param name="aiSelected">The currently selected AI option.</param>
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
    /// Updates the text of the message label and ensures its visibility.
    /// </summary>
    /// <param name="text">The text to be displayed on the message label.</param>
    private async void UpdateLabel(string text)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            this.MessageLabel.IsVisible = true;
            this.MessageLabel.Text = text;
        });
        await Task.Yield();
    }

    #endregion
}
public class LanguageToMicrosoftVoiceConverter : IValueConverter
{
    private static readonly Dictionary<string, string> LanguageVoiceMapping = new()
    {
        {"af", "af-ZA-AdriNeural"},
        {"ar", "ar-SA-HamedNeural"},
        {"bn", "bn-BD-NabanitaNeural"},
        {"bg", "bg-BG-BorislavNeural"},
        {"zh-Hans", "zh-CN-XiaochenNeural"},
        {"zh-Hant", "zh-HK-HiuGaaiNeural"},
        {"hr", "hr-HR-SreckoNeural"},
        {"cs", "cs-CZ-AntoninNeural"},
        {"da", "da-DK-ChristofferNeural"},
        {"nl", "nl-NL-ColetteNeural"},
        {"en", "en-US-JennyNeural"},
        {"fi", "fi-FI-NiinaNeural"},
        {"fr", "fr-FR-DeniseNeural"},
        {"de", "de-DE-AmalaNeural"},
        {"el", "el-GR-NestorasNeural"},
        {"he", "he-IL-AvriNeural"},
        {"hi", "hi-IN-SwaraNeural"},
        {"hu", "hu-HU-TamasNeural"},
        {"id", "id-ID-ArifNeural"},
        {"it", "it-IT-AntonellaNeural"},
        {"ja", "ja-JP-NanamiNeural"},
        {"ko", "ko-KR-SunHiNeural"},
        {"ms", "ms-MY-OsmanNeural"},
        {"fa", "fa-IR-DilaraNeural"},
        {"pl", "pl-PL-EwaNeural"},
        {"pt", "pt-PT-AntonioNeural"},
        {"pa", "pa-IN-IqraNeural"},
        {"ro", "ro-RO-CarmenNeural"},
        {"ru", "ru-RU-DmitryNeural"},
        {"sr", "sr-RS-NikolaNeural"},
        {"sk", "sk-SK-LukasNeural"},
        {"es", "es-ES-AbrilNeural"},
        {"sv", "sv-SE-MattiasNeural"},
        {"ta", "ta-IN-ValluvarNeural"},
        {"th", "th-TH-PhairojNeural"},
        {"tr", "tr-TR-AhmetNeural"},
        {"uk", "uk-UA-PolinaNeural"},
        {"ur", "ur-PK-AsadNeural"},
        {"vi", "vi-VN-NamNeural"}
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string languageCode && LanguageVoiceMapping.TryGetValue(languageCode, out var voice))
        {
            return voice;
        }
        return "Onbekende stem";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}