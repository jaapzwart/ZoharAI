using AVFoundation;

namespace ZoharBible;

/// <summary>
/// The ChatAnalysis class is a content page within the Zohar Bible application, designed
/// to retrieve and display a text-based analysis of a specific proverb.
/// </summary>
public partial class ChatAnalysis : ContentPage
{
    /// <summary>
    /// Holds the translated text returned from the translation service.
    /// This variable is used to store the content which is to be displayed
    /// in the ChatAnalysis text editor after being translated from the
    /// original response obtained from the AI service APIs.
    /// </summary>
    private string translatedText = "";
    
    /// <summary>
    /// The ChatAnalysis class is a content page within the Zohar Bible application.
    /// It provides functionality to retrieve and display a text-based analysis of a specific proverb.
    /// </summary>
    public ChatAnalysis()
    {
        InitializeComponent();
        if (GlobalVars.Amida_.Contains("Parshat"))
            this.messageLabel.Text = GlobalVars._pPortion;
        UpdateLabelCaller("Preparing first analysis...");
        ConfigureAudioSession();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(1000);  // Deze vertraging zorgt ervoor dat de pagina volledig is geladen.
        GlobalVars._IntroPage = false;
        await AfterAppearingLogic();
    }
    private async Task AfterAppearingLogic()
    {
        await OnButtonClicked(this, EventArgs.Empty);
        await UpdateLabelCaller("...");
    }

    /// <summary>
    /// Configures the audio session for the application, setting the category to Playback
    /// and activating the session. This is essential for proper audio functionality within the app,
    /// particularly for playback scenarios.
    /// </summary>
    private void ConfigureAudioSession()
    {
        try
        {
            var audioSession = AVAudioSession.SharedInstance();
            audioSession.SetCategory(AVAudioSessionCategory.Playback);
            //audioSession.SetMode(AVAudioSessionMode.SpokenAudio);
            audioSession.SetActive(true);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Failed to configure audio session: " + ex.Message, "OK");
        }
    }

    /// <summary>
    /// Event handler for button click events in the ChatAnalysis page.
    /// This method is triggered when a button is clicked, and it initiates the process
    /// of fetching the translated text analysis of a specified proverb.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, this is the Button that was clicked.</param>
    /// <param name="args">Provides data for the event.</param>
    private async Task OnButtonClicked(object sender, EventArgs args)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            await GetTranslatedText();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
        }
    }
    /// <summary>
    /// Retrieves and translates text-based analysis of a specific Jewish prayer or proverb.
    /// The text is fetched from an external API based on the selected AI service and
    /// parameters defined in global variables.
    /// </summary>
    /// <returns>Task that represents the asynchronous operation of fetching and translating the text.</returns>
    private async Task GetTranslatedText()
    {
        try
        {
            string responseText = "";
            string qq = "";
            
            if (GlobalVars._Dialogue)
            {
                GlobalVars._extraDialogueActionsForQuestion = " Formulate a final question as the LAST" +
                                                              " paragraph of your text based on your analysis.";
            }
            else
            {
                GlobalVars._extraDialogueActionsForQuestion = "";
            }
            #region Init Amida Shema and Proverb
            if (GlobalVars.Amida_.Contains("Amida") || GlobalVars.Amida_.Contains("Shema"))
            {
                qq = "Show all the prayers with content of the Jewish " + GlobalVars.Amida_ +
                     " and after that give a deep analysis of the Jewish" +
                     GlobalVars.Amida_ +
                     " and add to it how you can apply it to your behavior for the day, towards your family, friends and other people.";
                
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._extraDialogueActionsForQuestion +=
                        " On the content of the Jewish " + GlobalVars.Amida_ +
                        " and after that give a deep analysis of the Jewish" +
                        GlobalVars.Amida_ +
                        " and add to it how you can apply it to your behavior for the day";
                    qq += GlobalVars._extraDialogueActionsForQuestion;

                }
            }

            string qp = "";
            string hAdd = "";
            if (GlobalVars._pPortion.Contains("Horoscope"))
            {
                qp = "Give an analysis on " +
                            GlobalVars._pPortion + " for the current date" +
                            " and for the period of a " + GlobalVars.HPeriod;
                
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._extraDialogueActionsForQuestion +=
                        " On " +
                        GlobalVars._pPortion + " for the current date" +
                        " and for the period of a " + GlobalVars.HPeriod;
                    qp += GlobalVars._extraDialogueActionsForQuestion;

                }
                hAdd = "(" + GlobalVars._pPortion + " - " + GlobalVars.HPeriod + ")";
            }
            else if (GlobalVars._pPortion.Contains("Tarot"))
            {
                qp = "Give an analysis on the TarotCard " +
                     GlobalVars.theCardT + " on position " + GlobalVars.thePositionT +
                     " in a Tarot throw of 3 cards past, present and future.";
                
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._extraDialogueActionsForQuestion +=
                        " On the TarotCard " +
                        GlobalVars.theCardT + " on position " + GlobalVars.thePositionT +
                        " in a Tarot throw of 3 cards past, present and future.";
                    qp += GlobalVars._extraDialogueActionsForQuestion;

                }
            }
            else if (GlobalVars._pPortion.Contains("Proverbs"))
            {
                qp = "Give an analysis on " +
                     GlobalVars.ProverbToAnalyse +
                     " from out the perspective of the " +
                     GlobalVars.TypeOfProverbAnalysis;
                
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._extraDialogueActionsForQuestion += 
                        " Related to " +
                        GlobalVars.ProverbToAnalyse +
                        " from out the perspective of the " +
                        GlobalVars.TypeOfProverbAnalysis;
                    qp += GlobalVars._extraDialogueActionsForQuestion;
                }
            }
            else
            {
                qp = "Give an analysis on " +
                     GlobalVars.ProverbToAnalyse +
                     " from out the perspective of the " +
                     GlobalVars.TypeOfProverbAnalysis;
                
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._extraDialogueActionsForQuestion += 
                        " Related to " +
                        GlobalVars.ProverbToAnalyse +
                        " from out the perspective of the " +
                        GlobalVars.TypeOfProverbAnalysis;
                    qp += GlobalVars._extraDialogueActionsForQuestion;

                }
            }
            #endregion
            
            #region Amida and Shema
            if (GlobalVars.Amida_.Contains("Amida") || GlobalVars.Amida_.Contains("Shema"))
            {
                if (GlobalVars.AiSelected.Contains("ChatGPT"))
                {
                    responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                        Secrets.RESTAPI + @"ChatGPT/"
                        + qq + GlobalVars._extraDialogueActionsForQuestion);
                    responseText = GlobalVars.DialogueCleaner(responseText) + '\n' + '\n';
                    if (GlobalVars._Dialogue)
                    {
                        GlobalVars._DialogueQuestion = GetQuestion(responseText);
                        responseText += await DoDialogue(GlobalVars._DialogueQuestion);
                    }
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                    translatedText = GlobalVars.DialogueCleaner(translatedText);
                    this.ChatAnalysisText.Text = "\n\n" + "CHATGPT: " + "\n\n" + translatedText.TrimStart() + '\n' + '\n';
                }
                else if (GlobalVars.AiSelected.Contains("GroK"))
                {
                    responseText = await Secrets.GetGrok(qq + GlobalVars._extraDialogueActionsForQuestion);
                    responseText = GlobalVars.DialogueCleaner(responseText) + '\n' + '\n';
                    if (GlobalVars._Dialogue)
                    {
                        GlobalVars._DialogueQuestion = GetQuestion(responseText);
                        responseText += await DoDialogue(GlobalVars._DialogueQuestion);
                    }
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                    translatedText = GlobalVars.DialogueCleaner(translatedText);
                    this.ChatAnalysisText.Text = "\n\n" + "GROK: " + "\n\n" + translatedText.TrimStart() + '\n' + '\n';
                }
                else if (GlobalVars.AiSelected.Contains("Gemini"))
                {
                    responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                        Secrets.RESTAPI + @"Google/"
                        + qq + GlobalVars._extraDialogueActionsForQuestion);
                    responseText = GlobalVars.DialogueCleaner(responseText) + '\n' + '\n';
                    if (GlobalVars._Dialogue)
                    {
                        GlobalVars._DialogueQuestion = GetQuestion(responseText);
                        responseText += await DoDialogue(GlobalVars._DialogueQuestion);
                    }
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                    translatedText = GlobalVars.DialogueCleaner(translatedText);
                    this.ChatAnalysisText.Text = "\n\n" + "GOOGLE: " + "\n\n" + translatedText.TrimStart() + '\n' + '\n';
                }
                else if (GlobalVars.AiSelected.Contains("AllAI"))
                {
                    responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                        Secrets.RESTAPI + @"ChatGPT/"
                        + qq + GlobalVars._extraDialogueActionsForQuestion);
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                    translatedText = GlobalVars.DialogueCleaner(translatedText);
                    this.ChatAnalysisText.Text = "\n\n" + "CHATGPT: " + "\n\n" + translatedText.TrimStart() + '\n' + '\n';
                    responseText = await Secrets.GetGrok(qq + GlobalVars._extraDialogueActionsForQuestion);
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                    translatedText = GlobalVars.DialogueCleaner(translatedText);
                    this.ChatAnalysisText.Text += "\n\n" + "GROK: " + "\n\n" + translatedText.TrimStart() + '\n' + '\n';
                    responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                        Secrets.RESTAPI + @"ChatGPT/"
                        + qq + GlobalVars._extraDialogueActionsForQuestion);
                    translatedText = await Translator.TranslateTextToGiven(responseText);
                    translatedText = GlobalVars.DialogueCleaner(translatedText);
                    this.ChatAnalysisText.Text += "\n\n" + "GOOGLE: " + "\n\n" + translatedText.TrimStart();
                }
            }
            #endregion
            
            #region Proverb analysis
            else if (GlobalVars.AiSelected.Contains("ChatGPT"))
            {
                responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/"
                    + qp + GlobalVars._extraDialogueActionsForQuestion);
                responseText = GlobalVars.DialogueCleaner(responseText) + '\n' + '\n';
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._DialogueQuestion = GetQuestion(responseText);
                    responseText += "\n\n" + await DoDialogue(GlobalVars._DialogueQuestion);
                }
                
                translatedText = await Translator.TranslateTextToGiven(responseText);
                translatedText = GlobalVars.DialogueCleaner(translatedText);
                this.ChatAnalysisText.Text = "\n\n" + "CHATGPT: " + hAdd + "\n\n" + translatedText.TrimStart()
                    + "\n\n";
            }
            else if (GlobalVars.AiSelected.Contains("GroK"))
            {
                responseText = await Secrets.GetGrok(qp + GlobalVars._extraDialogueActionsForQuestion);
                responseText = GlobalVars.DialogueCleaner(responseText) + '\n' + '\n';
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._DialogueQuestion = GetQuestion(responseText);
                    responseText += await DoDialogue(GlobalVars._DialogueQuestion);
                }
                
                translatedText = await Translator.TranslateTextToGiven(responseText);
                translatedText = GlobalVars.DialogueCleaner(translatedText);
                this.ChatAnalysisText.Text = "\n\n" + "GROK: " + hAdd + "\n\n" + translatedText.TrimStart();
            }
            else if (GlobalVars.AiSelected.Contains("Gemini"))
            {
                responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(Secrets.RESTAPI + @"Google/"
                    + qp + GlobalVars._extraDialogueActionsForQuestion);
                responseText = GlobalVars.DialogueCleaner(responseText) + '\n' + '\n';
                
                if (GlobalVars._Dialogue)
                {
                    GlobalVars._DialogueQuestion = GetQuestion(responseText);
                    responseText += await DoDialogue(GlobalVars._DialogueQuestion);
                }

                translatedText = await Translator.TranslateTextToGiven(responseText);
                translatedText = GlobalVars.DialogueCleaner(translatedText);
                this.ChatAnalysisText.Text = "\n\n" + "GOOGLE: " + hAdd + "\n\n" + translatedText.TrimStart();
            }
            else if (GlobalVars.AiSelected.Contains("AllAI"))
            {
                this.ChatAnalysisText.Text = "All AI ANalysis: " + '\n' + '\n';
                responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/" + qp + GlobalVars._extraDialogueActionsForQuestion);
                translatedText = await Translator.TranslateTextToGiven(responseText);
                translatedText = GlobalVars.DialogueCleaner(translatedText);
                this.ChatAnalysisText.Text += "\n\n" + "CHATGPT: " + hAdd + "\n\n" + translatedText.TrimStart() + '\n' + '\n';
                responseText = await Secrets.GetGrok(qp + GlobalVars._extraDialogueActionsForQuestion);
                translatedText = await Translator.TranslateTextToGiven(responseText);
                translatedText = GlobalVars.DialogueCleaner(translatedText);
                this.ChatAnalysisText.Text += "\n\n" + "GROK: " + hAdd + "\n\n" + translatedText.TrimStart() + '\n' + '\n';
                responseText = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"Google/" + qp + GlobalVars._extraDialogueActionsForQuestion);
                translatedText = await Translator.TranslateTextToGiven(responseText);
                translatedText = GlobalVars.DialogueCleaner(translatedText);
                this.ChatAnalysisText.Text += "\n\n" + "GOOGLE: " + hAdd + "\n\n" + translatedText.TrimStart();
            }
            #endregion
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred while fetching or translating the text: " + ex.Message,
                "OK");
        }
        
    }

    /// <summary>
    /// Processes a given question by generating a dialogue response using the configured AI models.
    /// Depending on the AI models selected, it updates the dialogue with responses from different AI sources,
    /// and prepares additional questions to continue the dialogue.
    /// </summary>
    /// <param name="question">The initial question to which the dialogue is responding.</param>
    /// <returns>A string containing the dialogue responses from the selected AI models.</returns>
    // TODO: Down code is a bit Groggy: needs refactoring.
    private async Task<string> DoDialogue(string question)
    {
        string _dialogue = "";
        string qOrg = " End your analysis with a new question and ONLY a question as the final" +
            " paragraph.";
        if (GlobalVars.AiSelected.Contains("ChatGPT"))
        {
            await UpdateLabelCaller("Preparing dialogue Grok");
            await Task.Delay(1000);
            
            GlobalVars._DialogueQuestion = GetQuestion(question);
            await CreateQuestionBasedOnAIProvider(GlobalVars._DialogueQuestion, "ChatGPT");

            
            _dialogue = "\n\n" + "GROK - " + "\n\n"  +
                        "Building further on the part " + '\n' +
                        GlobalVars._DialogueQuestion + "\n\n" + await Secrets.GetGrok(GlobalVars._DialogueQuestion
                            + qOrg + GlobalVars._extraDialogueActionsForQuestion);
            _dialogue = GlobalVars.DialogueCleaner(_dialogue);
            
            GlobalVars._DialogueQuestion = GetQuestion(_dialogue);
            await CreateQuestionBasedOnAIProvider(GlobalVars._DialogueQuestion, "Grok");
            
            await UpdateLabelCaller("Starting with Google dialogue.");
            await Task.Delay(1000);
            _dialogue += "\n\n" + "GOOGLE - " + "\n\n" +
                "Building further on the part " + '\n' +
                GlobalVars._DialogueQuestion + "\n\n" + await GlobalVars.GetHttpReturnFromAPIRestLink(
                Secrets.RESTAPI + @"Google/"
                                + GlobalVars._DialogueQuestion
                                + GlobalVars._extraDialogueActionsForQuestion);
            _dialogue = GlobalVars.DialogueCleaner(_dialogue);
            await UpdateLabelCaller("Done.");
        }
        if (GlobalVars.AiSelected.Contains("GroK"))
        {
            UpdateLabelCaller("Preparing dialogue ChatGPT");
            await Task.Delay(1000);
            
            GlobalVars._DialogueQuestion = GetQuestion(question);
            await CreateQuestionBasedOnAIProvider(GlobalVars._DialogueQuestion, "Grok");
            
            string __dialogue = await GlobalVars.GetHttpReturnFromAPIRestLink(
                Secrets.RESTAPI + @"ChatGPT/"
                                + GlobalVars._DialogueQuestion + qOrg
                                + GlobalVars._extraDialogueActionsForQuestion);
            
            __dialogue = GlobalVars.DeleteAllBeforeFirstCapitalLetter(__dialogue);
            __dialogue = "\n\n" + "CHATGPT - " + "\n\n" + 
                         "Building further on the part " + '\n' + 
                         GlobalVars._DialogueQuestion + "\n\n"
                         + __dialogue;
            _dialogue = GlobalVars.DialogueCleaner(__dialogue);
            
            GlobalVars._DialogueQuestion = GetQuestion(_dialogue);
            await CreateQuestionBasedOnAIProvider(GlobalVars._DialogueQuestion, "ChatGPT");

            UpdateLabelCaller("Starting with Google dialogue.");
            await Task.Delay(1000);
            _dialogue += "\n\n" + "GOOGLE - " + "\n\n"  +
                         "Building further on the part " + '\n' +
                         GlobalVars._DialogueQuestion + "\n\n" + await GlobalVars.GetHttpReturnFromAPIRestLink(
                             Secrets.RESTAPI + @"Google/"
                                             + GlobalVars._DialogueQuestion
                                             + GlobalVars._extraDialogueActionsForQuestion);
            _dialogue = GlobalVars.DialogueCleaner(_dialogue);
        }
        if (GlobalVars.AiSelected.Contains("Gemini"))
        {
            await UpdateLabelCaller("Preparing dialogue Grok");
            await Task.Delay(1000);
            
            GlobalVars._DialogueQuestion = GetQuestion(question);
            await CreateQuestionBasedOnAIProvider(GlobalVars._DialogueQuestion, "Gemini");
            
            _dialogue = "\n\n" + "GROK - " + "\n\n"  +
                        "Building further on the part " + '\n' +
                        GlobalVars._DialogueQuestion  + "\n\n" + await Secrets.GetGrok(GlobalVars._DialogueQuestion
                        + qOrg + GlobalVars._extraDialogueActionsForQuestion);
            _dialogue = GlobalVars.DialogueCleaner(_dialogue);
            
            GlobalVars._DialogueQuestion = GetQuestion(_dialogue);
            await CreateQuestionBasedOnAIProvider(GlobalVars._DialogueQuestion, "Grok");
            
            await UpdateLabelCaller("Starting with ChatGPT dialogue.");
            await Task.Delay(1000);
            string __dialogue = await GlobalVars.GetHttpReturnFromAPIRestLink(
                Secrets.RESTAPI + @"ChatGPT/"
                                + GlobalVars._DialogueQuestion
                                + GlobalVars._extraDialogueActionsForQuestion);
            __dialogue = GlobalVars.DeleteAllBeforeFirstCapitalLetter(__dialogue);
            __dialogue = GlobalVars.DialogueCleaner(__dialogue);
            __dialogue = "\n\n" + "CHATGPT - " + "\n\n" + 
                         "Building further on the part " + '\n' + 
                         GlobalVars._DialogueQuestion + "\n\n" +  __dialogue;
            _dialogue += __dialogue;
            await UpdateLabelCaller("Done.");
        }

        return _dialogue;
    }

    private async Task CreateQuestionBasedOnAIProvider(string _dialogue, string AIProvider)
    {
        if (!_dialogue.EndsWith('?'))
        {
            string qText = "Create a question and ONLY a question without any other text based on ";
            if (AIProvider.Contains("ChatGPT"))
            {
                GlobalVars._DialogueQuestion = GetQuestion(_dialogue);
                GlobalVars._DialogueQuestion = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"ChatGPT/" +
                    qText + GlobalVars._DialogueQuestion);
                GlobalVars._DialogueQuestion = GetQuestion(GlobalVars._DialogueQuestion);
            }
            else if (AIProvider.Contains("Grok"))
            {
                GlobalVars._DialogueQuestion = GetQuestion(_dialogue);
                GlobalVars._DialogueQuestion = await Secrets.GetGrok(
                    qText + GlobalVars._DialogueQuestion);
                GlobalVars._DialogueQuestion = GetQuestion(GlobalVars._DialogueQuestion);
            }
            else if (AIProvider.Contains("Gemini"))
            {
                GlobalVars._DialogueQuestion = GetQuestion(_dialogue);
                GlobalVars._DialogueQuestion = await GlobalVars.GetHttpReturnFromAPIRestLink(
                    Secrets.RESTAPI + @"Google/" +
                    qText + GlobalVars._DialogueQuestion);
                GlobalVars._DialogueQuestion = GetQuestion(GlobalVars._DialogueQuestion);
            }
        }
    }

    private async Task UpdateLabelCaller(string text)
    {
        try
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.messageLabel.Text = text;
            });
            await Task.Yield();
        }
        catch (Exception ex)
        {
            
        }
    }
    private string GetQuestion(string input)
    {
        // Split the string into lines
        var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Find the last line that starts with a capital letter
        var lastCapitalizedLine = lines
            .LastOrDefault(line => !string.IsNullOrWhiteSpace(line) && char.IsUpper(line.TrimStart()[0]));
       
        return lastCapitalizedLine;
    }

    /// <summary>
    /// Handles the Click event of the Speak button, converting the translated text
    /// to speech using the TextToSpeechService.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button.</param>
    /// <param name="e">Event arguments associated with the Click event.</param>
    private async void OnSpeakButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            await GlobalVars.ttsService.ConvertTextToSpeechAsync(translatedText);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred while trying to speak the text: " + ex.Message, "OK");
        }
    }

    /// <summary>
    /// Handles the event when the stop speaking button is clicked to stop the Text-to-Speech service.
    /// </summary>
    /// <param name="sender">The sender object that raised the event.</param>
    /// <param name="e">Event data associated with the event.</param>
    private async void OnStopSpeakButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await GlobalVars.SetClickedColor(sender);
            await GlobalVars.ttsService.StopSpeakingAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred while trying to stop the speech: " + ex.Message, "OK");
        }
    }
}

/// <summary>
/// The Translatetext class provides functionality to translate given text
/// into the language specified in the GlobalVars configuration.
/// </summary>
public static class Translatetext
{
    /// <summary>
    /// Retrieves the translated text for a given input string.
    /// </summary>
    /// <param name="tt">The input text that needs to be translated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the translated text.</returns>
    public static async Task<string> GetTranslatedText(string tt)
    {
        try
        {
            string translatedText = await Translator.TranslateTextToGiven(tt);
            return translatedText;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error",
                "An error occurred while translating the text: " + ex.Message, "OK");
            throw;
        }
    }
}