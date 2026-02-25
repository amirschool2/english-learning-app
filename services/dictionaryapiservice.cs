using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EnglishLearningApp.Services
{
    public static class DictionaryApiService
    {
        private static readonly HttpClient _client = new HttpClient();
        private const string BasePath = "https://api.dictionaryapi.dev/api/v2/entries/en/";

        /// <summary>
        /// Get definition for a word from Free Dictionary API
        /// </summary>
        public static async Task<string?> GetDefinitionAsync(string word)
        {
            try
            {
                var response = await _client.GetAsync($"{BasePath}{word}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonString = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(jsonString);

                if (data.Count == 0)
                    return null;

                var definition = data[0]["meanings"]?[0]?["definitions"]?[0]?["definition"]?.ToString();
                return definition ?? "[Definition not available]";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dictionary API error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get pronunciation URL for a word
        /// </summary>
        public static async Task<string?> GetPronunciationUrlAsync(string word)
        {
            try
            {
                var response = await _client.GetAsync($"{BasePath}{word}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonString = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(jsonString);

                if (data.Count == 0)
                    return null;

                var phonetics = data[0]["phonetics"];
                if (phonetics == null)
                    return null;

                foreach (var phonetic in phonetics)
                {
                    var audio = phonetic["audio"]?.ToString();
                    if (!string.IsNullOrEmpty(audio))
                        return audio;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Pronunciation API error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get full word data
        /// </summary>
        public static async Task<JObject?> GetWordDataAsync(string word)
        {
            try
            {
                var response = await _client.GetAsync($"{BasePath}{word}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonString = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(jsonString);

                return data.Count > 0 ? (JObject)data[0] : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dictionary API error: {ex.Message}");
                return null;
            }
        }
    }
}
