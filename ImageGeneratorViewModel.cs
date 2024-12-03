using System.ComponentModel; // For implementing the INotifyPropertyChanged interface.
using System.Text; // For encoding text data.
using System.Windows.Input; // For creating commands in MVVM patterns.
using Newtonsoft.Json; // For JSON serialization and deserialization.
using System.Text.Json; // Alternative for JSON serialization/deserialization.
using OpenAI.API.Models; // For interacting with OpenAI models.
using JsonSerializer = System.Text.Json.JsonSerializer; // Alias for JSON serializer from System.Text.Json.

namespace ZoharBible
{
    /// <summary>
    /// ViewModel for interacting with OpenAI's DALL-E API to generate images.
    /// Implements the INotifyPropertyChanged interface to notify the UI of property changes.
    /// </summary>
    public class MainViewModelChatGPT : INotifyPropertyChanged
    {
        private string _generatedImageUrl; // Stores the URL of the generated image.
        private readonly HttpClient _httpClient; // HTTP client for making API requests.

        /// <summary>
        /// Constructor initializes the ViewModel and sets up the GenerateImageCommand.
        /// </summary>
        public MainViewModelChatGPT()
        {
            _httpClient = new HttpClient(); // Instantiate the HTTP client.
            GenerateImageCommand = new Command(async () => await GenerateImageAsync()); // Command for generating images.
        }

        /// <summary>
        /// Event triggered when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// URL of the generated image. Notifies UI when changed.
        /// </summary>
        public string GeneratedImageUrl
        {
            get => _generatedImageUrl;
            set
            {
                _generatedImageUrl = value;
                OnPropertyChanged(nameof(GeneratedImageUrl)); // Notify UI of the property change.
            }
        }

        /// <summary>
        /// Command to initiate the image generation process.
        /// </summary>
        public ICommand GenerateImageCommand { get; }

        /// <summary>
        /// Asynchronous method to generate an image using OpenAI's DALL-E API.
        /// </summary>
        private async Task GenerateImageAsync()
        {
            // API key and endpoint fetched from Secrets for security.
            string apiKey = Secrets.DallE_key;
            string endpoint = Secrets.urlDalle;

            // Configure the HTTP client for authorization.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            // Request body for the DALL-E API call.
            var requestBody = new
            {
                model = "dall-e-3", // Specify the model version.
                prompt = GlobalVars.DallE_Image_string, // Prompt for the image generation.
                size = "1024x1024", // Resolution of the generated image.
                quality = "standard", // Quality level of the generated image.
                n = 1 // Number of images to generate.
            };

            // Serialize the request body to JSON format.
            string jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // Send the POST request to the DALL-E API endpoint.
                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                // If the response indicates success, process the response.
                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the response content.
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    // Extract the URL of the generated image from the response.
                    string imageUrl = result.data[0].url;
                    GeneratedImageUrl = imageUrl; // Update the property with the URL.
                }
                else
                {
                    // Handle unsuccessful API responses (e.g., errors, rate limits).
                    // For example: log the response or notify the user.
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues, deserialization errors).
                // For example: log the exception or show an error message to the user.
            }
        }

        /// <summary>
        /// Notifies the UI of a property value change.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Model class representing the response from the DALL-E API.
    /// </summary>
    public class ImageResponse
    {
        public List<ImageData> Data { get; set; } // List of generated image data.
    }

    /// <summary>
    /// Model class for individual image data in the DALL-E API response.
    /// </summary>
    public class ImageData
    {
        public string Url { get; set; } // URL of the generated image.
    }
}
