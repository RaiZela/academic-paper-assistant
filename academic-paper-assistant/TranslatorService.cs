using Azure.AI.Translation.Text;

namespace academic_paper_assistant;

public class TranslatorService
{
    private readonly TextTranslationClient _client;
    public TranslatorService(TextTranslationClient client)
    {
        _client = client;
    }

    public async Task<List<string>> TranslateAsync(List<string> inputTexts, string targetLanguage)
    {
        var response = await _client.TranslateAsync(targetLanguage, inputTexts);
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
}
