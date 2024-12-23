using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ZoharBible;

/// <summary>
/// Provides functionality for creating video clips with lip-sync using external API services.
/// </summary>
public static class DoVideoClipLipSync
{
    public static async Task<string> getStringFromJson(string jjson)
    {
        string jsonString = jjson;

        // Parse the JSON string
        var jsonDocument = JsonDocument.Parse(jsonString);

        // Extract the video_id
        string videoId = jsonDocument.RootElement
            .GetProperty("data")
            .GetProperty("video_id")
            .GetString();

        return videoId;
    }
    public static async Task<string> checkVideoID(string _key, string _id)
    {
       
            // Replace with your API key and video ID
            string apiKey = _key;
            string videoId = _id;
            string url = $"https://api.heygen.com/v1/video_status.get?video_id={videoId}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Add required headers
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

                    // Make the GET request
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Read and output the response
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return responseString;
                    }
                    else
                    {
                        return response.StatusCode.ToString();
                        
                    }
                }
                catch (Exception ex)
                {
                    return "An error occurred:";
                   
                }
            }
        
    }
    public static async Task<string> CreateHeyGenVideoAvatar(string _key, string AIText)
    {
        string videoID = "";
        // Replace with your API key
        string apiKey = _key;
        string url = "https://api.heygen.com/v2/video/generate";
        string _tt = AIText;
            
        // JSON payload
        string jsonPayload = $@"
        {{
            ""video_inputs"": [
                {{
                    ""character"": {{
                        ""type"": ""avatar"",
                        ""avatar_id"": ""fb0f46758c5d4298a73d23d52c5f24a4"",
                        ""avatar_style"": ""normal""
                    }},
                    ""voice"": {{
                        ""type"": ""text"",
                        ""input_text"": ""{_tt}"",
                        ""voice_id"": ""26b2064088674c80b1e5fc5ab1a068ec"",
                        ""speed"": 1.1
                    }}
                }}
            ],
            ""dimension"": {{
                ""width"": 1280,
                ""height"": 720
            }}
        }}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Set up the request
                client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Make the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Read and output the response
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return responseString; 
                }
                else
                {
                    return response.StatusCode.ToString();
                    
                }
            }
            catch (Exception ex)
            {
                return "An error occurred:";
               
            }
        }
    
    }
    public static async Task<string> CreateHeyGenTalkingPhotoVideo(string _apiKey, string AIText, string _photoID)
    {
        string apiKey = _apiKey;
        string url = "https://api.heygen.com/v2/video/generate";
        string _tt = AIText;
        // JSON payload

        // JSON payload
        string jsonPayload = $@"
        {{
            ""video_inputs"": [
                {{
                    ""character"": {{
                        ""type"": ""talking_photo"",
                        ""talking_photo_id"": ""{_photoID}""
                    }},
                    ""voice"": {{
                        ""type"": ""text"",
                        ""input_text"": ""{_tt}"",
                        ""voice_id"": ""7v0Lk9jbYKjKYpjoYhK6""
                    }},
                    ""background"": {{
                        ""type"": ""color"",
                        ""value"": ""#FAFAFA""
                    }}
                }}
            ]
        }}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Set up the request
                client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Make the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Read and output the response
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return responseString; // Successfully received a response
                }
                else
                {
                    return $"Error: {response.StatusCode}\n{responseString}";
                }
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }

    /// <summary>
    /// Creates a video clip with lip-sync for non-animated avatars. 
    /// </summary>
    /// <param name="whatToSay">The text to be spoken in the video.</param>
    /// <returns>A task that represents the asynchronous operation, returning the ID of the created video or "Found" if the video already exists.</returns>
    public static async Task<string> CreateClipAvatar(string whatToSay)
    {
        // Clean the input text for dialogue usage
        whatToSay = GlobalVars.DialogueCleaner(whatToSay);
        string fileName = "";
        
        // Determine the file name based on who is speaking
        if(GlobalVars.videoTalked.Contains("Bill")) fileName = "TalkedBill.mp4";
        if(GlobalVars.videoTalked.Contains("Elon")) fileName = "TalkedElon.mp4";
        if(GlobalVars.videoTalked.Contains("Google")) fileName = "TalkedGoogle.mp4";
        if(GlobalVars.videoTalked.Contains("Interactive")) fileName = "TalkedInteractive.mp4";
        
        string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        
        if (GlobalVars.waitingLoopScreenDo)
        {
            GlobalVars.videoFileExists = true;
            GlobalVars.videoTalked = "waiting";
            GlobalVars.videoFilePath = Path.Combine(FileSystem.AppDataDirectory, GlobalVars.waitingLoopScreen);
            return "Found";
        }
        
        // Check if the video file already exists, unless it's an interactive clip
        if (!fileName.Contains("TalkedInteractive") && File.Exists(filePath))
        {
            GlobalVars.videoFileExists = true;
            GlobalVars.videoFilePath = filePath;
            return "Found";
        }

        // Setup API request for video creation
        var options = new RestClientOptions("https://api.d-id.com/talks");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("authorization", Secrets.ID_Key);
        
        // JSON body for the API request
        string voicefromMainscreen = GlobalVars.LanguageChoosenByFullName;
        string manualMaileVoidForTom = "en-GB-RyanNeural";
        string jsonBody = $"{{\"source_url\":\"https://throwcards.azurewebsites.net/tomm.png\",\"script\":{{\"type\":\"text\",\"subtitles\":\"false\",\"provider\":{{\"type\":\"microsoft\",\"voice_id\":\"{manualMaileVoidForTom}\"}},\"input\":\"{whatToSay}\"}},\"config\":{{\"fluent\":\"false\",\"pad_audio\":\"0.0\"}},\"user_data\":\"What to talk\"}}";
        request.AddJsonBody(jsonBody, false);
        var response = await client.PostAsync(request);
        
        // Parse response and get the video ID
        JObject json = JObject.Parse(response.Content);
        string idValue = json["id"]?.ToString();
        
        return await Task.FromResult(idValue);
    }

    /// <summary>
    /// Creates an animated video clip using a specified avatar image.
    /// </summary>
    /// <param name="whatToSay">The text to be spoken in the video (not used in this method but kept for consistency).</param>
    /// <returns>A task that represents the asynchronous operation, returning the ID of the created animation or "Found" if the video already exists.</returns>
    public static async Task<string> CreateAnimationAvatar(string whatToSay)
    {
        string fileName = "";
        if (GlobalVars.videoTalked.Contains("Bill")
            || GlobalVars.videoTalked.Contains("Elon")
            || GlobalVars.videoTalked.Contains("Google"))
        {
            fileName = "TalkedAnimated.mp4";
        }
        else
            fileName = "loading.mp4";
        
        

        string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        // Check if animation already exists
        if (File.Exists(filePath))
        {
            GlobalVars.videoFileExists = true;
            GlobalVars.videoFilePath = filePath;
            return "Found";
        }

        // Setup API request for animation creation
        var options = new RestClientOptions("https://api.d-id.com/animations");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("authorization", Secrets.ID_Key);
        
        // JSON body for animation request
        string jsonBody = "{\"source_url\":\"https://d-id-public-bucket.s3.us-west-2.amazonaws.com/alice.jpg\"}";
        request.AddJsonBody(jsonBody, false);
        var response = await client.PostAsync(request);
        
        // Parse response and get the animation ID
        JObject json = JObject.Parse(response.Content);
        string idValue = json["id"]?.ToString();
        
        return await Task.FromResult(idValue);
    }

    /// <summary>
    /// Retrieves the URL of a previously created video clip or animation.
    /// </summary>
    /// <param name="theClip">The ID of the clip to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation, returning the URL of the video clip or "Not found" if unsuccessful.</returns>
    public static async Task<string> GetVideoClip(string theClip)
    {
        try
        {
            RestClientOptions options;
            if(GlobalVars.anim)
                options = new RestClientOptions("https://api.d-id.com/animations/" + theClip);
            else
                options = new RestClientOptions("https://api.d-id.com/talks/" + theClip);
            
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", Secrets.ID_Key);

            string resultUrl = "Not found";
            int x = 0;
            // Retry mechanism to get the video URL
            for (int retry = 0; retry < 10; retry++)
            {
                var response = await client.GetAsync(request);
                await Task.Delay(3000);

                if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);

                    if (jsonDocument.RootElement.TryGetProperty("result_url", out JsonElement resultUrlElement))
                    {
                        resultUrl = resultUrlElement.GetString() ?? "Not found";

                        if (resultUrl != "Not found")
                        {
                            break; // Exit loop if URL is found
                        }
                    }

                    x++;
                }

                await Task.Delay(2000); // Delay before retrying
            }
            return resultUrl;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in loop creating video:" + '\n' +
                              e.Message + '\n' + e.StackTrace);
            throw;
        }
    }

    public static async Task<string> GetCredits()
    {
        var options = new RestClientOptions("https://api.d-id.com/credits");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("authorization", Secrets.ID_Key);
        var response = await client.GetAsync(request);

        return response.Content;
    }

    /// <summary>
    /// Downloads a video from a given URL to local storage.
    /// </summary>
    /// <param name="url">The URL of the video to download.</param>
    /// <returns>A task that represents the asynchronous operation, returning the local file path of the downloaded video.</returns>
    public static async Task<string> DownloadVideoAsync(string url)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to download video.");

        string fileName = "";
        if (GlobalVars.videoTalked.Contains("Interactive")) fileName = "TalkedInteractive.mp4";
        if (GlobalVars.videoTalked.Contains("Bill"))
        {
            fileName = GlobalVars.anim ? "TalkedAnimation.mp4" : "TalkedBill.mp4";
        }
        if(GlobalVars.videoTalked.Contains("Elon"))
        {
            fileName = GlobalVars.anim ? "TalkedAnimation.mp4" : "TalkedElon.mp4";
        }
        if(GlobalVars.videoTalked.Contains("Google"))
        {
            fileName = GlobalVars.anim ? "TalkedAnimation.mp4" : "TalkedGoogle.mp4";
        }
        
        string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = File.Create(filePath))
        {
            await stream.CopyToAsync(fileStream);
        }

        return filePath;
    }

    /// <summary>
    /// Returns the file path of an existing video file.
    /// </summary>
    /// <param name="_Video">The name of the video file.</param>
    /// <returns>The full path to the video file in the cache directory.</returns>
    public static async Task<string> PlayExistingVideo(string _Video)
    {
        string fileName = _Video;
        string filePath = "";
        var l = FileSystem.CacheDirectory.Length;
        filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        return filePath;
    }
}