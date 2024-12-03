using Color = Microsoft.Maui.Graphics.Color;

namespace ZoharBible
{
    /// <summary>
    /// Manages theme colors for the application's UI elements.
    /// </summary>
    public class Themes
    {
        // Default color values in hex format
        private static string _pageBackground = "#1E3A5F";
        private static string _buttonBackground = "#61A0D7";
        private static string _buttonBorder = "#FFFFFF";
        private static string _buttonText = "#FFFFFF";
        private static string _analyzeBackground = "#00FF00";
        private static string _analyzeText = "#000000";
        private static string _messageLabelBackground = "#00008B";
        private static string _messageLabelText = "#00FF00";

        // Public properties for accessing colors, initialized with default values
        public static Color PageBackgroundC { get; set; } = Color.FromHex(_pageBackground);
        public static Color ButtonBackgroundC { get; set; } = Color.FromHex(_buttonBackground);
        public static Color ButtonBorderC { get; set; } = Color.FromHex(_buttonBorder);
        public static Color ButtonTextC { get; set; } = Color.FromHex(_buttonText);
        public static Color _AnalyzeBack { get; set; } = Color.FromHex(_analyzeBackground);
        public static Color _AnalyzeText { get; set; } = Color.FromHex(_analyzeText);
        public static Color MessageLabelBackgroundC { get; set; } = Color.FromHex(_messageLabelBackground);
        public static Color MessageLabelTextC { get; set; } = Color.FromHex(_messageLabelText);

        /// <summary>
        /// Adjusts theme colors based on a mood value to dynamically change the UI appearance.
        /// </summary>
        /// <param name="_mood">A value between 10 and 100 representing mood, where 100 is the most vibrant.</param>
        public static void SetMoodColors(int _mood)
        {
            // Clamp mood between 10 (low) and 100 (high)
            _mood = Math.Clamp(_mood, 10, 100);

            // Calculate brightness scale from mood
            float scale = (_mood - 10) / 90f; // Normalized scale between 0 and 1

            // Adjust button background color
            float backgroundRed = 30 + scale * 200; // Red channel (30 to 230)
            float backgroundGreen = 50 + scale * 180; // Green channel (50 to 230)
            float backgroundBlue = 80 + scale * 150; // Blue channel (80 to 230)

            string buttonBackgroundHex = ConvertToHex(backgroundRed / 255f, backgroundGreen / 255f, backgroundBlue / 255f);
            ButtonBackgroundC = Color.FromHex(buttonBackgroundHex);

            // Adjust button text color to ensure it's lighter
            float textRed = Math.Min(backgroundRed + 70, 255); // Ensure it doesn't exceed 255
            float textGreen = Math.Min(backgroundGreen + 70, 255);
            float textBlue = Math.Min(backgroundBlue + 70, 255);

            string buttonTextHex = ConvertToHex(textRed / 255f, textGreen / 255f, textBlue / 255f);
            ButtonTextC = Color.FromHex(buttonTextHex);

            // Adjust page background
            float pageBackgroundRed = 10 + scale * 100;
            float pageBackgroundGreen = 20 + scale * 100;
            float pageBackgroundBlue = 30 + scale * 100;

            string pageBackgroundHex = ConvertToHex(pageBackgroundRed / 255f, pageBackgroundGreen / 255f, pageBackgroundBlue / 255f);
            PageBackgroundC = Color.FromHex(pageBackgroundHex);

            // Adjust border color
            float borderRed = Math.Max(textRed - 50, 0);
            float borderGreen = Math.Max(textGreen - 50, 0);
            float borderBlue = Math.Max(textBlue - 50, 0);

            string buttonBorderHex = ConvertToHex(borderRed / 255f, borderGreen / 255f, borderBlue / 255f);
            ButtonBorderC = Color.FromHex(buttonBorderHex);

            // Adjust analyze background color
            float analyzeBackRed = 40 + scale * 150;
            float analyzeBackGreen = 120 + scale * 100;
            float analyzeBackBlue = 60 + scale * 140;

            string analyzeBackHex = ConvertToHex(analyzeBackRed / 255f, analyzeBackGreen / 255f, analyzeBackBlue / 255f);
            _AnalyzeBack = Color.FromHex(analyzeBackHex);

            // Adjust analyze text color
            float analyzeTextRed = Math.Min(analyzeBackRed + 80, 255);
            float analyzeTextGreen = Math.Min(analyzeBackGreen + 80, 255);
            float analyzeTextBlue = Math.Min(analyzeBackBlue + 80, 255);

            string analyzeTextHex = ConvertToHex(analyzeTextRed / 255f, analyzeTextGreen / 255f, analyzeTextBlue / 255f);
            _AnalyzeText = Color.FromHex(analyzeTextHex);

            // Adjust message label background
            float messageBackRed = 50 + scale * 180; // A different range for distinct color
            float messageBackGreen = 100 + scale * 120;
            float messageBackBlue = 150 + scale * 90;

            string messageLabelBackgroundHex = ConvertToHex(messageBackRed / 255f, messageBackGreen / 255f, messageBackBlue / 255f);
            MessageLabelBackgroundC = Color.FromHex(messageLabelBackgroundHex);

            // Adjust message label text color
            float messageTextRed = Math.Min(messageBackRed + 80, 255);
            float messageTextGreen = Math.Min(messageBackGreen + 80, 255);
            float messageTextBlue = Math.Min(messageBackBlue + 80, 255);

            string messageLabelTextHex = ConvertToHex(messageTextRed / 255f, messageTextGreen / 255f, messageTextBlue / 255f);
            MessageLabelTextC = Color.FromHex(messageLabelTextHex);
        }

        /// <summary>
        /// Converts RGB color values to a hexadecimal color string.
        /// </summary>
        /// <param name="red">Red component, from 0 to 1.</param>
        /// <param name="green">Green component, from 0 to 1.</param>
        /// <param name="blue">Blue component, from 0 to 1.</param>
        /// <returns>A string representing the color in hex format #RRGGBB.</returns>
        static string ConvertToHex(float red, float green, float blue)
        {
            int r = (int)(red * 255);
            int g = (int)(green * 255);
            int b = (int)(blue * 255);

            // Format as #RRGGBB
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        /// <summary>
        /// Sets the theme back to predefined standard colors.
        /// </summary>
        public static void SetStandardTheme()
        {
            _pageBackground = "#1E3A5F";
            _buttonBackground = "#61A0D7";
            _buttonBorder = "#FFFFFF";
            _buttonText = "#FFFFFF";
            _analyzeBackground = "#00FF00";
            _analyzeText = "#33FF33";
            _messageLabelBackground = "#FFFFFF";
            _messageLabelText = "#000000";

            PageBackgroundC = Color.FromHex(_pageBackground);
            ButtonBackgroundC = Color.FromHex(_buttonBackground);
            ButtonBorderC = Color.FromHex(_buttonBorder);
            ButtonTextC = Color.FromHex(_buttonText);
            _AnalyzeBack = Color.FromHex(_analyzeBackground);
            _AnalyzeText = Color.FromHex(_analyzeText);
            MessageLabelBackgroundC = Color.FromHex(_messageLabelBackground);
            MessageLabelTextC = Color.FromHex(_messageLabelText);
        }
    }
}