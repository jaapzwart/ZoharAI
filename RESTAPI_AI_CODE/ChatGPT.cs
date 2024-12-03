//-------------------------------------------------------
// The code below is behind a REST API GET call in Azure.
// It calls for ChatGPT giving it a string to work on.
// This is called in ZoharBible when ChatGPT is chosen.
// You have to make it work for your own environment.
// The code is example code.
//-------------------------------------------------------

/*
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.OpenApi;
using Nest;
using OpenAI_API.Completions;
using OpenAI.API.Completions;


/// <summary>
/// The ChatGPT class contains methods for translating text using the OpenAI GPT-3.5-turbo API
/// and utilizes Azure Blob Storage for storing and reading data.
/// </summary>
namespace ZoharBible.RESTAPI_AI_CODE
{
    public class ChatGPT
    {
        /// <summary>
        /// Receives a text to translate, makes a translation request to OpenAI, and returns the result.
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <returns>A string with the translated response or an error message.</returns>
        [HttpGet("{textToTranslate}")]
        public async Task<string> Get(string textToTranslate)
        {
            try
            {
                // Create a unique date string for the file name
                string getDate = DateTime.Now.Hour.ToString("d") + DateTime.Now.Minute.ToString("d") +
                                 DateTime.Now.Second.ToString("d") + DateTime.Now.Millisecond.ToString("d");
                // Remove non-alphabetic characters from the text
                string cleanedString = Regex.Replace(textToTranslate, @"[^a-zA-Z\s]+", "");
                // Replace spaces with underscores (_) in the cleaned text
                cleanedString = cleanedString.Replace(" ", "_");
                // Write the text to a Blob with the generated file name
                await writeFileToBlob(textToTranslate, getDate + " " + cleanedString + ".txt");
                // Instance of OpenAI API with specified key
                OpenAI_API openAI = new OpenAI_API.OpenAIAPI("KEY");
                // Read the temperature setting for ChatGPT from the Blob ("temperatureChatGPT.txt")
                string temperature = readFileFromBlob("temperatureChatGPT.txt", "BLOBNAME");
                double _temperature = Convert.ToDouble(temperature);
                // Read the max tokens number for ChatGPT from the Blob ("maxTokensChatGPT.txt")
                string maxtokensChatGPT = readFileFromBlob("maxTokensChatGPT.txt", "BLOBNAME");
                int _maxtokensChatGPT = Convert.ToInt32(maxtokensChatGPT);
                // Create a CompletionRequest with the appropriate settings
                CompletionRequest completion = new CompletionRequest
                {
                    Prompt = textToTranslate,
                    MaxTokens = _maxtokensChatGPT,
                    Temperature = _temperature,
                    Model = "gpt-3.5-turbo-instruct" // Specify the model ID for GPT-3.5-turbo
                };
                // Request a completion from OpenAI
                var result = await openAI.Completions.CreateCompletionAsync(completion);
                // Process the completed results
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
                {
                    return "No results from BlackBeltBible AI.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// Writes the text to Azure Blob storage with the given file name.
        /// </summary>
        /// <param name="writeToBlobKentekenControle">The text to write.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string indicating whether the operation was successful or not.</returns>
        private async Task<string> writeFileToBlob(string writeToBlobKentekenControle, string fileName)
        {
            try
            {
                // Initialize the CloudStorageAccount with a connection string
                string connS = "CONNECTION STRING";
                CloudStorageAccount account = CloudStorageAccount.Parse(connS);
                // Create a BlobClient to communicate with the Blob service
                var blobClient = account.CreateCloudBlobClient();
                // Get a reference to the container (create it if it doesn't exist)
                var blobContainer = blobClient.GetContainerReference("chatgpt");
                await blobContainer.CreateIfNotExistsAsync();
                WebClient wc = new WebClient();
                using (Stream fs = wc.OpenWrite("URL" + fileName))
                {
                    TextWriter tw = new StreamWriter(fs);
                    CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);
                    // Upload the text to the Blob
                    await blockBlob.UploadTextAsync(writeToBlobKentekenControle);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// Reads the content of a file from Azure Blob storage.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="blobber">The name of the Blob container.</param>
        /// <returns>The content of the file as a string.</returns>
        public static string readFileFromBlob(string fileName, string blobber)
        {
            try
            {
                // Initialize the CloudStorageAccount with a connection string
                string connS = "CONNECTION STRING";
                CloudStorageAccount account = CloudStorageAccount.Parse(connS);
                // Create a BlobClient to communicate with the Blob service
                var blobClient = account.CreateCloudBlobClient();
                // Get a reference to the container (create it if it doesn't exist)
                var blobContainer = blobClient.GetContainerReference(blobber);
                blobContainer.CreateIfNotExistsAsync();
                // Get a reference to the blob file and download the content
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{fileName}");
                string contents = blob.DownloadTextAsync().Result;
                return contents;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
    }
}
*/