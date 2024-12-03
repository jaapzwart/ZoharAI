using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZoharBible;

/// <summary>
/// View model for managing the transcription of audio, implementing the INotifyPropertyChanged interface for UI updates.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private string _result;

    /// <summary>
    /// Gets or sets the transcription result, notifying any subscribers when the property changes.
    /// </summary>
    public string Result
    {
        get { return _result; }
        set
        {
            _result = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Command to initiate audio transcription.
    /// </summary>
    public ICommand TranscribeCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class, setting up the transcription command.
    /// </summary>
    public MainViewModel()
    {
        TranscribeCommand = new Command(async () => await TranscribeAudio());
    }

    /// <summary>
    /// Transcribes audio from a specified file and updates the <see cref="Result"/> property with the transcription.
    /// </summary>
    private async Task TranscribeAudio()
    {
        Result = await AzureSpeechToText.TranscribeAudioAsync("audiofile.wav", ["zoekwoord"] );
    }

    /// <summary>
    /// Event handler for when a property changes. This is used for data binding in UI frameworks.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed. Automatically set by the CallerMemberName attribute.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}