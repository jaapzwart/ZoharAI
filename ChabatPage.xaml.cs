namespace ZoharBible;

/// <summary>
/// Represents the Chabat page of the ZoharBible application.
/// </summary>
public partial class ChabatPage : ContentPage
{
    #region Constructor

    /// <summary>
    /// Represents the ChabatPage of the ZoharBible application. This page allows
    /// a user to enter a specific URL and load the corresponding web page within the app.
    /// </summary>
    public ChabatPage()
    {
        InitializeComponent();
    }
    public ChabatPage(string cc)
    {
        InitializeComponent();
        UrlEntry.Text = cc;
        OnLoadUrlClicked(null, null);
    }
    #endregion

    #region Event Handlers

    /// <summary>
    /// Event handler for the "Load URL" button click event.
    /// Loads a URL from the entry field into the WebView.
    /// If the URL contains specific keywords, it loads predefined URLs.
    /// Validates the URL before loading it in the WebView.
    /// Displays an alert if the URL is invalid.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">Event data containing information about the click event.</param>
    private async void OnLoadUrlClicked(object sender, EventArgs e)
    {
        // Get the URL from the entry
        await GlobalVars.SetClickedColor(sender);
        string url = UrlEntry.Text;
        if (UrlEntry.Text.Contains("zohar"))
            url = "https://zohar.com";
        if (UrlEntry.Text.Contains("kabbalah"))
            url = "https://www.chabad.org/kabbalah/article_cdo/aid/1270227/jewish/Daily-Zohar-Hok-LYisrael.htm";

        // Validate the URL
        if (!string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            // Load the URL in the WebView
            Browser.Source = url;
        }
        else
        {
            DisplayAlert("Invalid URL", "Please enter a valid URL.", "OK");
        }
    }

    /// <summary>
    /// Event handler that is triggered when the URL entry field gains focus.
    /// Clears the text in the entry field to allow the user to enter a new URL.
    /// </summary>
    /// <param name="sender">The source of the event, typically the Entry that received focus.</param>
    /// <param name="e">Event data containing information about the focus event.</param>
    private async void UrlEntry_Focused(object sender, FocusEventArgs e)
    {
        try
        {
            UrlEntry.Text = string.Empty; // Clear text when Entry gets focus
        }
        catch (Exception ex)
        {
            // Display an alert when an exception occurs
            await Application.Current.MainPage.DisplayAlert("Error", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    #endregion
}