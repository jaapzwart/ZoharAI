using AVFoundation;
using Foundation;
using NAudio.Wave;

namespace ZoharBible
{
    /// <summary>
    /// Defines the interface for audio recording, playback, and manipulation services.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Starts the audio recording process.
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Stops the ongoing audio recording.
        /// </summary>
        void StopRecording();

        /// <summary>
        /// Plays back the recorded audio.
        /// </summary>
        void PlayRecording();

        /// <summary>
        /// Stops the playback of the recorded audio.
        /// </summary>
        void StopPlayback();

        /// <summary>
        /// Returns the file path where the recording is saved.
        /// </summary>
        /// <returns>The path to the recorded audio file.</returns>
        string GetFilePath();

        /// <summary>
        /// Increases the sensitivity of the microphone for better audio capture.
        /// </summary>
        void IncreaseMicrophoneSensitivity();

        /// <summary>
        /// Converts an M4A audio file to WAV format.
        /// </summary>
        /// <param name="filePath">The path to the input M4A file.</param>
        /// <param name="outfilePath">The path where the WAV file will be saved.</param>
        void ConvertM4AtoWAV(string filePath, string outfilePath);
    }

    /// <summary>
    /// Implementation of the <see cref="IAudioService"/> interface for handling audio operations.
    /// </summary>
    public class AudioService : IAudioService
    {
        /// <summary>
        /// The file path where the WAV recording is saved.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The audio recorder instance used for recording audio.
        /// </summary>
        private AVAudioRecorder _recorder;

        /// <summary>
        /// The audio player instance used for playing back recorded audio.
        /// </summary>
        private AVAudioPlayer _player;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioService"/> class.
        /// </summary>
        public AudioService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "recording.wav");
        }

        /// <summary>
        /// Configures the audio session and starts recording audio to a WAV file.
        /// </summary>
        public void StartRecording()
        {
            var audioSession = AVAudioSession.SharedInstance();
            audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            audioSession.SetActive(true);

            // Update settings for WAV recording
            var settings = new AudioSettings
            {
                SampleRate = 44100, // Standard WAV sample rate
                Format = AudioToolbox.AudioFormatType.LinearPCM, // Linear PCM format for WAV
                NumberChannels = 1, // Mono recording
                AudioQuality = AVAudioQuality.High, // High-quality audio
                LinearPcmBitDepth = 16, // 16-bit samples
                LinearPcmBigEndian = false, // Little-endian byte order
                LinearPcmFloat = false // PCM integer format
            };

            // Create the recorder with the updated settings
            _recorder = AVAudioRecorder.Create(new NSUrl(_filePath, false), settings, out var error);

            if (error != null)
            {
                throw new Exception($"Failed to start recording: {error.LocalizedDescription}");
            }

            _recorder.Record();
        }

        /// <summary>
        /// Stops the recording process and disposes of the recorder.
        /// </summary>
        public void StopRecording()
        {
            if (_recorder != null)
            {
                _recorder.Stop();
                _recorder.Dispose();
                _recorder = null;
            }
        }

        /// <summary>
        /// Plays the recorded audio file if it exists.
        /// </summary>
        public void PlayRecording()
        {
            if (File.Exists(_filePath))
            {
                _player = AVAudioPlayer.FromUrl(new NSUrl(_filePath, false));
                _player.PrepareToPlay();
                _player.Play();
            }
            else
            {
                throw new FileNotFoundException("Recording file not found.");
            }
        }

        /// <summary>
        /// Stops the playback and disposes of the player.
        /// </summary>
        public void StopPlayback()
        {
            if (_player != null)
            {
                _player.Stop();
                _player.Dispose();
                _player = null;
            }
        }

        /// <summary>
        /// Configures the audio session to increase microphone sensitivity for better recording quality.
        /// </summary>
        public void IncreaseMicrophoneSensitivity()
        {
            var audioSession = AVAudioSession.SharedInstance();
            audioSession.SetCategory(AVAudioSessionCategory.Record, AVAudioSessionCategoryOptions.DefaultToSpeaker);
            audioSession.SetMode(AVAudioSession.ModeMeasurement, out var error);

            if (error != null)
            {
                throw new Exception(error.LocalizedDescription);
            }

            audioSession.SetActive(true);
        }

        /// <summary>
        /// Converts an M4A file to WAV format using NAudio libraries.
        /// </summary>
        /// <param name="inputFile">The path to the M4A file to convert.</param>
        /// <param name="outputFile">The path where the converted WAV file will be saved.</param>
        public void ConvertM4AtoWAV(string inputFile, string outputFile)
        {
            using var reader = new MediaFoundationReader(inputFile);
            using var waveStream = WaveFormatConversionStream.CreatePcmStream(reader);
            WaveFileWriter.CreateWaveFile(outputFile, waveStream);
        }

        /// <summary>
        /// Gets the file path where the current recording is saved.
        /// </summary>
        /// <returns>The path to the recording file.</returns>
        public string GetFilePath() => _filePath;
    }
}