using Azure.AI.Translation.Text;

namespace academic_paper_assistant.Services;

public class TranslatorService
{
    private readonly TextTranslationClient _client;
    public TranslatorService(TextTranslationClient client)
    {
        _client = client;
    }

    public async Task<List<string>> TranslateAsync(List<string> inputTexts, string targetLanguage)
    {
        var code = await GetCodeByNameAsync(targetLanguage);
        var response = await _client.TranslateAsync(code, inputTexts);
        var translations = new List<string>();

        foreach (var doc in response.Value)
        {
            foreach (var translation in doc.Translations)
            {
                translations.Add(translation.Text);
            }
        }
        return translations;
    }

    private async Task<Dictionary<string, string>> GetLanguageCodesAsync()
    {
        var response = await new HttpClient().GetFromJsonAsync<LanguageResponse>(
            "https://api.cognitive.microsofttranslator.com/languages?api-version=3.0&scope=translation");

        return response?.Translation
            .ToDictionary(k => k.Key, v => v.Value.Name) ?? new Dictionary<string, string>();
    }

    private async Task<string?> GetCodeByNameAsync(string languageName)
    {
        var langs = await GetLanguageCodesAsync();
        return langs.FirstOrDefault(l =>
            string.Equals(l.Value, languageName, StringComparison.OrdinalIgnoreCase)).Key;
    }

    private class LanguageResponse
    {
        public Dictionary<string, LanguageInfo> Translation { get; set; }
    }

    private class LanguageInfo
    {
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Dir { get; set; }
    }
}
