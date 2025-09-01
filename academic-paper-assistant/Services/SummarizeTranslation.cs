namespace academic_paper_assistant.Services;

public class SummarizeTranslation
{
    private readonly AcademicPaperService _academicPaperService;
    private readonly TranslatorService _translatorService;

    public SummarizeTranslation(AcademicPaperService academicPaperService, TranslatorService translatorService)
    {
        _academicPaperService = academicPaperService;
        _translatorService = translatorService;
    }
    public async Task<List<SummaryTranslationResult>> GetSummaryAndTranslation(string path, string language)
    {
        var summaries = await _academicPaperService.SummarizeText(path);
        var sentences = summaries.SelectMany(s => s.Sentences).ToList();

        var translatedSentences = await _translatorService.TranslateAsync(sentences, language);

        var result = sentences.Zip(translatedSentences, (original, translated) => new SummaryTranslationResult
        {
            OriginalSentence = original,
            TranslatedSentence = translated
        }).ToList();

        return result;
    }
}
