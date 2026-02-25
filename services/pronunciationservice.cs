using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NAudio.Wave;

namespace EnglishLearningApp.Services
{
    public static class PronunciationService
    {
        private static IWavePlayer? _wavePlayer;
        private static HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Play pronunciation audio from URL
        /// </summary>
        public static async Task PlayAudioAsync(string? audioUrl)
        {
            if (string.IsNullOrEmpty(audioUrl))
                return;

            try
            {
                _wavePlayer?.Dispose();
                _wavePlayer = new WaveOutEvent();

                // Download audio to memory
                byte[] audioData = await _httpClient.GetByteArrayAsync(audioUrl);

                // Create stream reader
                MemoryStream memoryStream = new MemoryStream(audioData);
                IWaveSource waveSource;

                // Try to read as MP3
                try
                {
                    waveSource = new MediaFoundationReader(memoryStream);
                }
                catch
                {
                    // Fallback to memory stream if MP3 fails
                    memoryStream.Position = 0;
                    waveSource = new WaveFileReader(memoryStream);
                }

                _wavePlayer.Init(waveSource);
                _wavePlayer.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio playback error: {ex.Message}");
            }
        }

        /// <summary>
        /// Stop audio playback
        /// </summary>
        public static void StopAudio()
        {
            _wavePlayer?.Stop();
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public static void Dispose()
        {
            _wavePlayer?.Dispose();
        }
    }
}
