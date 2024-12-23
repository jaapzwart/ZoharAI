using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using OpenAI.API.Completions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.AIPlatform.V1;

namespace ZoharBible;

/// <summary>
/// Provides methods for interacting with AI services, including Grok and ChatGPT.
/// </summary>
public class AIProviderReturns
{
    /// <summary>
    /// Sends a question to the Grok API and retrieves the response.
    /// </summary>
    /// <remarks>   Shadow, 7/12/2024. </remarks>
    /// <param name="question"> The question to be sent to the Grok API. </param>
    /// <returns>   A string response from the Grok API. </returns>
    public static async Task<string> GetGrok(string question)
    {
        string resultGrok = "";
        // Your API key and base URL
        string apiKey = Secrets.GrokKey;
        string baseURL = Secrets.GrokCompletionAddress;

        // Create the HTTP client
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            string gRok = "";
            // Define the request body
            var requestBody = new
            {
                model = "grok-beta",
                messages = new[]
                {
                    new { role = "system", content = Secrets.GrokRole },
                    new { role = "user", content = question }
                }
            };

            // Serialize the request body to JSON
            string json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make the POST request
            HttpResponseMessage response = await client.PostAsync(baseURL, content);

            // Read and output the response
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);
                resultGrok = result.choices[0].message.content;
                gRok = resultGrok;
                
            }
            else
            {
                resultGrok = response.StatusCode.ToString();
                gRok = resultGrok;
                
            }
        }
        return resultGrok;
        
    }
    #region ChatGPT and Google

    /// <summary>   Translates the given text using ChatGPT AI. </summary>
    /// <param name="textToTranslate"> The text to be translated. </param>
    /// <returns>   A Task that represents the asynchronous operation, containing the translated text. </returns>
    public static async Task<string> GetChatGPT(string textToTranslate)
        {
            try
            {
                string getDate = DateTime.Now.Hour.ToString("d") + DateTime.Now.Minute.ToString("d") +
                    DateTime.Now.Second.ToString("d") + DateTime.Now.Millisecond.ToString("d");
                string cleanedString = Regex.Replace(textToTranslate, @"[^a-zA-Z\s]+", "");
                cleanedString = cleanedString.Replace(" ", "_");
                await writeFileToBlob(textToTranslate, getDate + " " + cleanedString + ".txt");
                var openAI = new OpenAI.API.OpenAIAPI(Secrets.ChatGPTKey);
                string temperature = readFileFromBlob("temperatureChatGPT", Secrets.blobContainer);
                double _temperature = Convert.ToDouble(temperature);
                string maxtokensChatGPT = readFileFromBlob("maxTokensChatGPT.txt", Secrets.blobContainer);
                int _maxtokensChatGPT = Convert.ToInt32(maxtokensChatGPT);
    
                CompletionRequest completion = new CompletionRequest();
                completion.Prompt = textToTranslate;
                completion.MaxTokens = _maxtokensChatGPT;
                completion.Temperature = _temperature;
                completion.Model = Secrets.ChatGPTModel; // Set the model ID for GPT-3.5-turbo
                var result = await openAI.Completions.CreateCompletionAsync(completion);
                string answer = "";
                if (result != null)
                {
                    foreach (var item in result.Completions)
                    {
                        answer += "" + item.Text;
                    }
                    return answer;
                }
                else
                    return "No results from BlackBeltBible AI.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    
    private static async Task<string> writeFileToBlob(string writeToBlobKentekenControle, string fileName)
        {
            try
            {
                // Initialise client in a different place if you like
                string connS = Secrets.blobConnString;
                CloudStorageAccount account = CloudStorageAccount.Parse(connS);
                var blobClient = account.CreateCloudBlobClient();

                // Make sure container is there
                var blobContainer = blobClient.GetContainerReference("chatgpt");
                await blobContainer.CreateIfNotExistsAsync();

                WebClient wc = new WebClient();
                using (Stream fs = wc.OpenWrite(Secrets.ChatGPTEndpoint + fileName))
                {
                    TextWriter tw = new StreamWriter(fs);
                    CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(
                        fileName);
                    await blockBlob.UploadTextAsync(writeToBlobKentekenControle);
                    //tw.Flush();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>Reads a file from BLOB storage.</summary>
        /// <param name="fileName">The name of the file to be read.</param>
        /// <param name="blobber">The BLOB container reference where the file is stored.</param>
        /// <returns>The contents of the file from the BLOB storage. Returns "0" if an error occurs.</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string readFileFromBlob(string fileName, string blobber)
        {
            try
            {
                // Initialise client in a different place if you like
                string connS = Secrets.blobConnString;
                CloudStorageAccount account = CloudStorageAccount.Parse(connS);
                var blobClient = account.CreateCloudBlobClient();

                var blobContainer = blobClient.GetContainerReference(blobber);
                blobContainer.CreateIfNotExistsAsync();

                CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{fileName}");
                string contents = blob.DownloadTextAsync().Result;

                return contents;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
         public async Task<string> GetGoogle(string textToTranslate)
        {
            try
            {
                // Get the full path of the executable file
                string exePath = Assembly.GetExecutingAssembly().Location;

                // Get the directory of the executable file
                string directoryPath = Path.GetDirectoryName(exePath);
                
                string temperature = readFileFromBlob("temperatureChatGPT", Secrets.blobContainer);
                double _temperature = Convert.ToDouble(temperature);

                // 1. Vertex AI Configuration
                string projectId = Secrets.GoogleProjectID;     // Your Google Cloud Project ID
                string location = "us-central1";                // Your Vertex AI Model Location
                string publisher = "google";                    // Publisher of the model (e.g., "google")
                string modelId = "text-bison@001";              // ID of the Vertex AI Model
                string apiEndpoint = $"https://{location}-aiplatform.googleapis.com/v1/projects/{projectId}/locations/{location}/publishers/{publisher}/models/{modelId}:predict";
                var credentialsFilePath = Path.Combine(FileSystem.AppDataDirectory, Secrets.GoogleCredentialFile);
                // 2. Prepare the Request
                string question = textToTranslate;
                var requestData = new
                {
                    instances = new[]
                    {
                    new { prompt = question }
                },
                    parameters = new
                    {
                        temperature = _temperature,         // Slightly higher for creativity
                        maxOutputTokens = 1024,             // Increased token limit
                        topP = 0.95,                        // Nucleus sampling for diverse text
                        topK = 40                           // Top-k sampling for focused text
                    }
                };
                string jsonContent = JsonConvert.SerializeObject(requestData);

                // 3. Authenticate with OAuth 2.0 (Service Account)
                GoogleCredential credential = GoogleCredential.FromFile(credentialsFilePath)
                    .CreateScoped(PredictionServiceClient.DefaultScopes); // Adjust scopes if needed
                var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                // 4. Send the REST Request
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiEndpoint, content);

                // 4. Handle the Response
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    dynamic responseData = JsonConvert.DeserializeObject(responseJson);

                    string answer = responseData.predictions[0].content;
                    return answer;
                }
                else
                {
                    return "Failed: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
               
        }
       #endregion
}