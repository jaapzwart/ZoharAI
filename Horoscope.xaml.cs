namespace ZoharBible
{
    /// <summary>
    /// The Horoscope class is a part of the ZoharBible namespace and
    /// inherits from ContentPage. It represents the horoscope screen
    /// where users can select their sign and see corresponding information.
    /// This page initializes with the DayCheckBox set to checked.
    /// </summary>
    public partial class Horoscope : ContentPage
    {
        /// <summary>
        /// The Horoscope class is a part of the ZoharBible namespace and
        /// inherits from ContentPage. It provides functionality related
        /// to displaying and interacting with horoscope content.
        /// </summary>
        public Horoscope()
        {
            InitializeComponent();
            this.DayCheckBox.IsChecked = true;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GlobalVars._IntroPage = false;
        }

        /// <summary>
        /// A constant string representing the Alert Title used in the Horoscope view.
        /// </summary>
        private const string AlertTitle = "Horoscope";

        /// <summary>
        /// Handles the button click event for zodiac sign buttons.
        /// Updates the global variables and label, then navigates to the ChatAnalysis page.
        /// </summary>
        /// <param name="sender">The sender object. Should be a Button representing a zodiac sign.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnButtonClicked(object sender, EventArgs e)
        {
            await GlobalVars.SetClickedColor(sender);
            var button = sender as Button;
            if (button != null)
            {
                var zodiacSign = button.Text;
                // await DisplayAlert(AlertTitle, $"{zodiacSign} button clicked", "OK");
                GlobalVars._pPortion = "Horoscope " + zodiacSign;
                GlobalVars.Amida_ = "Horoscope";
                this.MessageLabel.IsVisible = true;
                UpdateLabel("Preparing Horoscope Analysis " + '\n'
                    + GlobalVars._pPortion 
                    + " - " + GlobalVars.HPeriod);
                await Task.Delay(1000);
                await Navigation.PushAsync(new ChatAnalysis());
                UpdateLabel("...");
            }
        }

        /// <summary>
        /// Updates the MessageLabel with the provided text, ensures the update is performed on the main thread, and handles any exceptions by displaying an error message.
        /// </summary>
        /// <param name="text">The text to display in the MessageLabel</param>
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
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        /// <summary>
        /// Handles the event when any of the checkboxes are checked or unchecked.
        /// Disables other checkboxes when one is checked and sets the HPeriod
        /// global variable to the corresponding period. Enables all checkboxes
        /// when none is checked.
        /// </summary>
        /// <param name="sender">The source of the event, typically a checkbox.</param>
        /// <param name="e">An <c>CheckedChangedEventArgs</c> that contains the event data.</param>
        private void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            string checker = "";
            if (sender is CheckBox selectedCheckBox && e.Value)
            {
                // Disable all other checkboxes
                if (selectedCheckBox != DayCheckBox)
                {
                    DayCheckBox.IsEnabled = false;
                }
                else
                {
                    checker = "Day";
                }

                if (selectedCheckBox != WeekCheckBox)
                {
                    WeekCheckBox.IsEnabled = false;
                }
                else
                {
                    checker = "Week";
                }

                if (selectedCheckBox != MonthCheckBox)
                {
                    MonthCheckBox.IsEnabled = false;
                }
                else
                {
                    checker = "Month";
                }

                if (selectedCheckBox != YearCheckBox)
                {
                    YearCheckBox.IsEnabled = false;
                }
                else
                {
                    checker = "Year";
                }

                GlobalVars.HPeriod = checker;
            }
            else
            {
                // Enable all checkboxes
                DayCheckBox.IsEnabled = true;
                WeekCheckBox.IsEnabled = true;
                MonthCheckBox.IsEnabled = true;
                YearCheckBox.IsEnabled = true;
            }
        }
    }
}