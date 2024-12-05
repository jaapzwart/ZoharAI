using System.Net;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ZoharBible;

/// <summary>
/// Provides functionality for creating video clips with lip-sync using external API services.
/// </summary>
public static class DoVideoClipLipSync
{
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
        string jsonBody = $"{{\"source_url\":\"https://d-id-public-bucket.s3.us-west-2.amazonaws.com/alice.jpg\",\"script\":{{\"type\":\"text\",\"subtitles\":\"false\",\"provider\":{{\"type\":\"microsoft\",\"voice_id\":\"{GlobalVars.LanguageChoosenByFullName}\"}},\"input\":\"{whatToSay}\"}},\"config\":{{\"fluent\":\"false\",\"pad_audio\":\"0.0\"}},\"user_data\":\"What to talk\"}}";
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