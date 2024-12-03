using System;
using System.Collections.Generic;
using Azure.AI.TextAnalytics;

namespace ZoharBible
{


    public class AngryWordDetector
    {
        private readonly TextAnalyticsClient _client;

        public AngryWordDetector(TextAnalyticsClient client)
        {
            _client = client;
        }

        public async Task AnalyzeText(string text)
        {
            // Perform sentiment analysis
            DocumentSentiment sentimentResult = await _client.AnalyzeSentimentAsync(text);
            // Display overall sentiment
            Console.WriteLine($"Overall Sentiment: {sentimentResult.Sentiment}");
            Console.WriteLine("Confidence Scores:");
            Console.WriteLine($"  Positive: {sentimentResult.ConfidenceScores.Positive}");
            Console.WriteLine($"  Neutral: {sentimentResult.ConfidenceScores.Neutral}");
            Console.WriteLine($"  Negative: {sentimentResult.ConfidenceScores.Negative}");

            if (sentimentResult.Sentiment == TextSentiment.Negative)
            {
                Console.WriteLine("Detected Negative Sentiment. Extracting key phrases...");
                string dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/"
                                    + "Tell the used that negative speech was detected" +
                                    " and that it is better to wait before continuing using" +
                                    " this Application because a peaceful state of mind is needed.");
                await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);
                // Extract key phrases from the text
                var keyPhrases = _client.ExtractKeyPhrases(text);

                // Filter phrases for potentially angry words
                var angryWords = GetAngryWords(keyPhrases.Value);

                // The potential abgry words.
                foreach (var word in angryWords)
                {
                    //Console.WriteLine($"  - {word}");
                }
            }
            else
            {
                Console.WriteLine("No strong negative sentiment detected.");
                string dSentiment = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/"
                                    + "Tell the user that no negative speech was detected" +
                                    " and that it is great to to continue using" +
                                    " this Application because a peaceful state of mind is needed" +
                                    " and it seems you have it.");
                await GlobalVars.ttsService.ConvertTextToSpeechAsync(dSentiment);
            }
        }

        private List<string> GetAngryWords(IEnumerable<string> keyPhrases)
        {
            // List of predefined angry or offensive words (expand as needed)
            var angryKeywords = new HashSet<string>
            {
                "hate", "stupid", "angry", "mad", "fool", "annoying", "idiot", "rage"
            };

            var detectedAngryWords = new List<string>();

            foreach (var phrase in keyPhrases)
            {
                foreach (var keyword in angryKeywords)
                {
                    if (phrase.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        detectedAngryWords.Add(phrase);
                        break;
                    }
                }
            }

            return detectedAngryWords;
        }
    }
}
