using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace ZoharBible;

/// <summary>
/// The <c>GlobalVars</c> class contains global variables that hold the application state and configurations.
/// </summary>
public static class GlobalVars
{
    public static string DallE_Image_string
    {
        get;
        set;
    } = "";

    public static string LanguageChoosenByFullName { get; set; } = "en-US-JennyNeural";
    
    public static bool AIInteractive { get; set; } = false;
    public static string AIInteractiveText { get; set; } = "";
    public static bool anim { get; set; } = false;
    public static string videoTalkedText { get; set; } = "Bill";
    public static bool videoFileExists { get; set; } = false;
    public static string videoFilePath { get; set; } = "";
    public static string videoTalked { get; set; } = "Elon";
    public static bool HeyComputerStartTalk { get; set; } = true;
    public static string  _extraDialogueActionsForQuestion { get; set; } = " Formulate a final question as the last paragraph based on your analysis.";
    public static string _dialogueTextReturn { get; set; } = "";
    public static bool _Dialogue { get; set; } = false;
    public static string _DialogueQuestion { get; set; } = "";
    public static double ChatGPTTemp { get; set; } = 0.1;
    public static bool _IntroPage { get; set; } = true;
    public static bool _StandardTheme { get; set; } = true;
    public static string moodOMeter { get; set; } = "80";
    public static bool firstThrow { get; set; } = true;
    public static string theCardT { get; set; } = "THE SUN";
    public static string thePositionT { get; set; } = "1";
    public static bool TAIInfo { get; set; } = false;

    public static bool TOnlineInfo { get; set; } = true;
    public static string cardChosen { get; set; } = "";
    
    public static string HPeriod { get; set; } = "Day";
    /// <summary>
    /// Gets or sets the chat analysis data.
    /// </summary>
    public static string ChatAnalysis { get; set; } = "";

    /// <summary>
    /// Specifies the type of analysis to be applied to a proverb.
    /// This could include perspectives such as Kabbalah, Zohar, Mishna, etc.
    /// Used across various components to tailor the analysis approach for a specific proverb.
    /// </summary>
    public static string TypeOfProverbAnalysis { get; set; } = "";

    /// <summary>
    /// Gets or sets the proverb text that is to be analyzed. This property is used
    /// to hold the specific proverb content that will undergo analysis within the application.
    /// </summary>
    public static string ProverbToAnalyse { get; set; } = "";

    /// <summary>
    /// Gets or sets a long string made up of 3000 repeated '#' characters.
    /// </summary>
    public static string longText { get; set; } = longString();

    /// <summary>
    /// A global variable used within the ZoharBible namespace to store a certain piece of data.
    /// </summary>
    public static string gRok { get; set; } = "";

    /// <summary>
    /// Gets or sets the AI service to be used for analysis.
    /// This property holds the name of the AI selected, such as "ChatGPT", "GroK", "Gemini", etc.
    /// It is utilized in various parts of the application to determine which AI service will process the requests.
    /// Default value is an empty string.
    /// </summary>
    public static string AiSelected { get; set; } = "";

    /// <summary>
    /// Represents a special identifier used for determining the type of content being analyzed or presented.
    /// This property can take values such as "Amida" or "Shema" to indicate specific types of analysis or presentation.
    /// </summary>
    public static string Amida_ { get; set; } = "";

    /// <summary>
    /// Represents the current portion or section being analyzed or processed by the application.
    /// This property is utilized in various contexts within the application to keep track of the portion
    /// of text that is currently being worked on, such as in text analysis or speech synthesis operations.
    /// </summary>
    public static string _pPortion { get; set; } = "";

    /// <summary>
    /// Gets or sets the language code used for translations and text-to-speech services.
    /// This property determines the target language for translation of text in the application,
    /// as well as the language and voice used in text-to-speech conversion.
    /// Default value is "English".
    /// </summary>
    public static string lLanguage_ { get; set; } = "en";

    /// <summary>
    /// Gets or sets the speech speed for text-to-speech conversion.
    /// </summary>
    /// <remarks>
    /// The value is represented as a string and should be between "0" and "100", where "100" denotes the slowest speech speed
    /// and "0" denotes the fastest. By default, the speed is set to "90".
    /// </remarks>
    public static string SpeechSpeed { get; set; } = "90";

    /// <summary>
    /// Gets or sets the string that determines whether the current text to navigate
    /// or analyze is from Proverbs or Psalms. This property influences the corresponding
    /// API endpoints and logic for text handling within the ZoharBible application.
    /// </summary>
    public static string _ProverbOrPsalm { get; set; } = "Proverbs";

    /// <summary>
    /// Provides access to the TextToSpeechService instance used for
    /// converting text into spoken audio using the Azure Text-to-Speech service.
    /// </summary>
    public static TextToSpeechService ttsService { get; set; } = new TextToSpeechService(
        Secrets.azureApiKey, Secrets.azureRegion);

    /// <summary>
    /// Generates a string consisting of a specified character repeated a specified number of times.
    /// </summary>
    /// <returns>
    /// A string with a specified character repeated 3000 times.
    /// </returns>
    private static string longString()
    {
        char character = '#'; // The character you want to repeat
        int repeatCount = 3000; // The number of times to repeat the character

        string repeatedString = new string(character, repeatCount);
        return repeatedString;
    }
    public static string DialogueCleaner(string _dialogue)
    {
        _dialogue = _dialogue.Replace(":", " ").Replace("(", " ").Replace(")", " ")
            .Replace("-", " ").Replace("*", "").Replace(" -", " ").Replace(" - ", " ")
            .Replace("***", "")
            .Replace("###", "")
            .Replace("**", "").Replace("*", "");

        _dialogue = Regex.Replace(_dialogue, @"[^\S\r\n,.]+", " ").Trim();
        
        return _dialogue;
    }
    public static async Task SetClickedColor(object? sender)
    {
        var button = sender as Button;

        if (GlobalVars._StandardTheme)
        {
            if (button != null)
            {
                button.IsEnabled = false;
                button.Background = new SolidColorBrush(Colors.LightGray);
                await Task.Delay(1000);
                button.IsEnabled = true;
                button.Background = new SolidColorBrush(Microsoft.Maui.Graphics.Color.Parse("#61A0D7"));
            }
        }
        else if (_IntroPage)
        {
            if (button != null)
            {
                button.IsEnabled = false;
                button.Background = new SolidColorBrush(Colors.LightGray);
                await Task.Delay(1000);
                button.IsEnabled = true;
                button.Background = new SolidColorBrush(Microsoft.Maui.Graphics.Color.Parse("#61A0D7"));
            }
        }
        else
        {
            if (button != null)
            {
                button.IsEnabled = false;
                button.Background = new SolidColorBrush(Colors.LightGray);
                // Wacht 1 seconde (1000 milliseconden)
                await Task.Delay(1000);
                button.IsEnabled = true;
                button.Background = Themes.ButtonBackgroundC;
            }
        }
    }
    /// <summary>
    /// Fetches the response string from a specified REST API URL.
    /// </summary>
    /// <param name="theLinkAPI">The URL of the REST API to fetch the response from.</param>
    /// <returns>The response string from the REST API, or an error message if an exception occurs.</returns>
    public static async Task<string> GetHttpReturnFromAPIRestLink(string theLinkAPI)
    {
        // This method has some troubles getting the string from the REST API in good format.
        try
        {
            var responseSimple = new WebClient().DownloadString(theLinkAPI);

            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(theLinkAPI);

            WebResponse response = request.GetResponse();
            string responseText = "";
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
            }

            GlobalVars.ChatAnalysis = "Analysis";
            return responseText;

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    /// <summary>
    /// Uploads a text string to an Azure Blob Storage and returns a status message.
    /// </summary>
    /// <param name="toWrite">The text string to be uploaded to the blob storage.</param>
    /// <param name="fileName">The name of the file to be created in the blob storage.</param>
    /// <returns>A string indicating success or an error message.</returns>
    public static async Task<string> writeFileToBlob(string toWrite, string fileName)
    {
        try
        {
            // Initialise client in a different place if you like
            string connS = Secrets._blobConnection;
            CloudStorageAccount account = CloudStorageAccount.Parse(connS);
            var blobClient = account.CreateCloudBlobClient();

            // Make sure container is there
            var blobContainer = blobClient.GetContainerReference(Secrets._blobReference);
            await blobContainer.CreateIfNotExistsAsync();

            WebClient wc = new WebClient();
            using (Stream fs = wc.OpenWrite(Secrets._blobTemp + fileName))
            {
                TextWriter tw = new StreamWriter(fs);
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(
                    fileName);
                await blockBlob.UploadTextAsync(toWrite);
                //tw.Flush();
            }
            return "Success";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static string DeleteAllBeforeFirstCapitalLetter(string input)
    {
        // Regular expression to match the first capital letter and everything after it
        const string pattern = @"[A-Z].*";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);
        Match match = regex.Match(input);

        return match.Success ? input.Substring(match.Index) : input;
    }
}


/// <summary>
/// Provides methods to translate text to a specified language.
/// </summary>
public static class Translator
{
    /// <summary>
    /// The subscription key required to authenticate API requests.
    /// This key is used for interacting with various services and APIs that require a subscription for access.
    /// Typically, the subscription key is a unique string provided by the service provider.
    /// </summary>
    private static readonly string subscriptionKey = Secrets.subscriptionKey;

    /// <summary>
    /// The endpoint for accessing the translation API service.
    /// </summary>
    private static readonly string endpoint = Secrets.enddpoint;

    /// <summary>
    /// Represents the region information required for making API calls to the translation service.
    /// This value is used as the subscription region in the HTTP request headers when
    /// interacting with the translation API.
    /// </summary>
    private static readonly string location = Secrets.llocation;

    /// <summary>
    /// Translates the provided text to the language specified in the global settings.
    /// </summary>
    /// <param name="inputText">The text to be translated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the translated text.</returns>
    public static async Task<string> TranslateTextToGiven(string inputText)
    {
        string route = "/translate?api-version=3.0&from=en&to=" + GlobalVars.lLanguage_;

        object[] body = new object[] { new { Text = inputText } };
        var requestBody = JsonConvert.SerializeObject(body);

        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage())
        {
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endpoint + route);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", location);

            HttpResponseMessage response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            var translationResult = JsonConvert.DeserializeObject<TranslationResult[]>(result);
            return translationResult[0].Translations[0].Text;
        }
    }

    /// <summary>
    /// Represents the result of a translation operation.
    /// </summary>
    private class TranslationResult
    {
        /// <summary>
        /// Gets or sets the translation results from the API response.
        /// </summary>
        public Translation[] Translations { get; set; }
    }

    /// <summary>
    /// Represents a translation result with the translated text.
    /// </summary>
    private class Translation
    {
        /// <summary>
        /// Gets or sets the text content used in translation results.
        /// </summary>
        /// <value>
        /// A string representing the translated text.
        /// </value>
        public string Text { get; set; }
    }
}

/// <summary>
/// The TextToSpeechService class provides functionality to convert text to speech using Azure's Text-to-Speech API.
/// </summary>
public class TextToSpeechService
{
    /// <summary>
    /// Handles text-to-speech synthesis operations utilizing the Microsoft Cognitive Services Speech SDK.
    /// </summary>
    private readonly SpeechSynthesizer _synthesizer;

    /// <summary>
    /// Provides text-to-speech capabilities using Microsoft Cognitive Services.
    /// </summary>
    public TextToSpeechService(string azureApiKey, string azureRegion)
    {
        var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
        config.SetProperty("speech-synthesis-speed", "2.0");
        _synthesizer = new SpeechSynthesizer(config);
    }

    /// <summary>
    /// Converts the given text to speech asynchronously using Microsoft's Cognitive Services.
    /// </summary>
    /// <param name="text">The text string that needs to be converted to speech.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ConvertTextToSpeechAsyncOld(string text)
    {
        var result = await _synthesizer.SpeakTextAsync(text);
        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            // Synthesis successful
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Console.WriteLine($"Speech synthesis canceled: {cancellation.Reason}");
            Console.WriteLine($"Error details: {cancellation.ErrorDetails}");
        }
    }

    /// Asynchronously stops the speech synthesis process.
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    public async Task StopSpeakingAsync()
    {
        await _synthesizer.StopSpeakingAsync();
    }

    /// <summary>
    /// Converts the provided text to speech asynchronously using specific voice parameters based on global settings.
    /// </summary>
    /// <param name="text">The text to be converted to speech.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ConvertTextToSpeechAsync(string text)
    {
        int transformedSpeed = 100 - Convert.ToInt32(GlobalVars.SpeechSpeed.Substring(0, 2));
        
        String language = GlobalVars.lLanguage_;
        string voiceName = language switch
        {
            "af" => "af-ZA-AdriNeural",          // Afrikaans
            "ar" => "ar-SA-ZariyahNeural",       // Arabic
            "bn" => "bn-IN-BashkarNeural",       // Bengali
            "bg" => "bg-BG-IvanNeural",          // Bulgarian
            "zh-Hans" => "zh-CN-XiaoxiaoNeural", // Chinese Simplified
            "zh-Hant" => "zh-TW-HsiaoChenNeural",// Chinese Traditional
            "hr" => "hr-HR-SreckoNeural",        // Croatian
            "cs" => "cs-CZ-VlastaNeural",        // Czech
            "da" => "da-DK-ChristelNeural",      // Danish
            "nl" => "nl-NL-ColetteNeural",       // Dutch
            "en" => "en-US-JennyNeural",         // English
            "fi" => "fi-FI-HarriNeural",         // Finnish
            "fr" => "fr-FR-DeniseNeural",        // French
            "de" => "de-DE-KatjaNeural",         // German
            "el" => "el-GR-NestorasNeural",      // Greek
            "he" => "he-IL-AvriNeural",          // Hebrew
            "hi" => "hi-IN-MadhurNeural",        // Hindi
            "hu" => "hu-HU-NoemiNeural",         // Hungarian
            "id" => "id-ID-ArdiNeural",          // Indonesian
            "it" => "it-IT-ElsaNeural",          // Italian
            "ja" => "ja-JP-KeitaNeural",         // Japanese
            "ko" => "ko-KR-SunHiNeural",         // Korean
            "ms" => "ms-MY-OsmanNeural",         // Malay
            "fa" => "fa-IR-DilaraNeural",        // Persian
            "pl" => "pl-PL-ZofiaNeural",         // Polish
            "pt" => "pt-BR-FranciscaNeural",     // Portuguese
            "pa" => "pa-IN-GurdeepNeural",       // Punjabi
            "ro" => "ro-RO-EmilNeural",          // Romanian
            "ru" => "ru-RU-DmitryNeural",        // Russian
            "sr" => "sr-RS-NicholasNeural",      // Serbian
            "sk" => "sk-SK-LukasNeural",         // Slovak
            "es" => "es-ES-ElviraNeural",        // Spanish
            "sv" => "sv-SE-MattiasNeural",       // Swedish
            "ta" => "ta-IN-PallaviNeural",       // Tamil
            "th" => "th-TH-AcharaNeural",        // Thai
            "tr" => "tr-TR-SedaNeural",          // Turkish
            "uk" => "uk-UA-PolinaNeural",        // Ukrainian
            "ur" => "ur-PK-AsadNeural",          // Urdu
            "vi" => "vi-VN-LienNeural",          // Vietnamese
            _ => "en-US-JennyNeural"             // Standaard stem
        };
        string ssml = $@"
            <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
                <voice name='{voiceName}'>
                    <prosody rate='-{transformedSpeed}%'>{text}</prosody>
                </voice>
            </speak>";
        var result = await _synthesizer.SpeakSsmlAsync(ssml);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            // Synthesis successful
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Console.WriteLine($"Speech synthesis canceled: {cancellation.Reason}");
            Console.WriteLine($"Error details: {cancellation.ErrorDetails}");
        }
    }
}

