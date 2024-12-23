using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ZoharBible;

/// <summary>
/// Represents the introductory page of the application, where users can adjust settings before navigating to other pages.
/// </summary>
public partial class Intro : ContentPage
{
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Intro"/> class, setting up initial UI states and global variables.
    /// </summary>
    public Intro()
    {
        InitializeComponent();
        GlobalVars._StandardTheme = true;
        GlobalVars._IntroPage = true;
        this.DialogueCheckBox.IsChecked = GlobalVars._Dialogue;
    }
   
    /// <summary>
    /// Called when the page appears, updates UI elements to reflect current settings.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.MessageLabel.Text = "...";
        GlobalVars._IntroPage = true;
        this.DialogueCheckBox.IsChecked = GlobalVars._Dialogue;
    }

    /// <summary>
    /// Handles changes in the creativity slider, updating the global temperature setting for ChatGPT.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments containing the new value of the slider.</param>
    private void OnCreativitySliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        double newValue = e.NewValue;
        GlobalVars.ChatGPTTemp = newValue;
        // Update the text of the corresponding label to reflect the new slider value
        CreativitySliderValueLabel.Text = $"Creativity: {newValue:F1}";
    }

    /// <summary>
    /// Navigates to the starter page, saving the current creativity setting and applying theme changes if necessary.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnNavigateToStarterPageClicked(object sender, EventArgs e)
    {
        await GlobalVars.writeFileToBlob(GlobalVars.ChatGPTTemp.ToString("0.0", CultureInfo.InvariantCulture),
            "temperatureChatGPT");
        if(this.StandardThemeCheckBox.IsChecked == false)
            Themes.SetMoodColors(Convert.ToInt32(GlobalVars.moodOMeter));
        await GlobalVars.SetClickedColor(sender);
        UpdateLabel("Opening Starting Page");
        await Task.Delay(1000);
        await Navigation.PushAsync(new StartPage());
    }

    /// <summary>
    /// Navigates to the audio test page, updating the theme if not using the standard one.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnNavigateToTestAudioPageClicked(object sender, EventArgs e)
    {
        if(this.StandardThemeCheckBox.IsChecked == false)
            Themes.SetMoodColors(Convert.ToInt32(GlobalVars.moodOMeter));
        await GlobalVars.SetClickedColor(sender);
        UpdateLabel("Opening Test Audio Page");
        await Task.Delay(1000);
        await Navigation.PushAsync(new TestAudioRecording());
    }

    /// <summary>
    /// Updates the message label on the UI thread with the provided text.
    /// </summary>
    /// <param name="text">The text to display in the message label.</param>
    private async void UpdateLabel(string text)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            this.MessageLabel.IsVisible = true;
            this.MessageLabel.Text = text;
        });
        await Task.Yield();
    }

    /// <summary>
    /// Handles changes to the mood slider, rounding and setting the mood value globally.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments containing the new value of the slider.</param>
    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        int roundedValue = (int)Math.Round(e.NewValue / 10.0) * 10;
        GlobalVars.moodOMeter = $"{roundedValue}";
        SliderValueLabel.Text = $"Slider Value: {roundedValue}";
    }

    /// <summary>
    /// Responds to changes in the standard theme checkbox, switching between standard and mood-based themes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments indicating whether the checkbox is checked.</param>
    private void OnStandardThemeCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            GlobalVars._StandardTheme = true;
            Themes.SetStandardTheme();
        }
        else
        {
            GlobalVars._StandardTheme = false;
            Themes.SetMoodColors(Convert.ToInt32(GlobalVars.moodOMeter));
        }
    }

    /// <summary>
    /// Handles changes to the dialogue mode checkbox, toggling dialogue mode in global settings.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments indicating whether the checkbox is checked.</param>
    private void DialogueCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        GlobalVars._Dialogue = e.Value;
    }
}