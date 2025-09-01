using academic_paper_assistant.Models;
using Azure.AI.TextAnalytics;

namespace academic_paper_assistant.Services;

public class AcademicPaperService
{
    private readonly TextAnalyticsClient _textAnalyticsClient;
    public AcademicPaperService(TextAnalyticsClient textAnalyticsClient)
    {
        _textAnalyticsClient = textAnalyticsClient;
    }
    public async Task<List<ExtractedSummary>> SummarizeText(string pdfPath)
    {
        var document = ExtractPdfText(pdfPath);
        var text = CleanExtractedText(document);
        var input = new List<string>
        {
            text
        };

        TextAnalyticsActions actions = new TextAnalyticsActions()
        {
            ExtractiveSummarizeActions = new List<ExtractiveSummarizeAction>() { new ExtractiveSummarizeAction() }
        };

        List<ExtractedSummary> extractedSummary = new List<ExtractedSummary>();
        AnalyzeActionsOperation operation = await _textAnalyticsClient.StartAnalyzeActionsAsync(input, actions);
        await operation.WaitForCompletionAsync();
        await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
        {
            IReadOnlyCollection<ExtractiveSummarizeActionResult> summaryResults = documentsInPage.ExtractiveSummarizeResults;

            foreach (ExtractiveSummarizeActionResult summaryActionResults in summaryResults)
            {
                if (summaryActionResults.HasError)
                {
                    extractedSummary.Add(new ExtractedSummary
                    {
                        ErrorCode = summaryActionResults.Error.ErrorCode.ToString(),
                        Message = summaryActionResults.Error.Message
                    });
                    continue;
                }

                foreach (ExtractiveSummarizeResult documentResults in summaryActionResults.DocumentsResults)
                {
                    if (documentResults.HasError)
                    {
                        extractedSummary.Add(new ExtractedSummary
                        {
                            ErrorCode = documentResults.Error.ErrorCode.ToString(),
                            Message = documentResults.Error.Message
                        });


                        continue;
                    }

                    var sentences = new List<string>();
                    foreach (ExtractiveSummarySentence sentence in documentResults.Sentences)
                    {
                        sentences.Add(sentence.Text);
                    }
                    extractedSummary.Add(new ExtractedSummary
                    {

                        ExtractedDescription = $"  Extracted the following {documentResults.Sentences.Count} sentence(s):",
                        Sentences = sentences
                    });
                }
            }
        }
        return extractedSummary;
    }
    private string ExtractPdfText(string pdfPath)
    {
        PdfDocument pdf = PdfDocument.FromFile(pdfPath);
        string extractedText = pdf.ExtractAllText();
        //string extractedText = pdf.ExtractAllText(0);
        return extractedText;
    }
    private string CleanExtractedText(string text)
    {
        var lines = text.Split('\n')
            .Where(l => !l.Contains("ironpdf.com/licensing"))
            .Where(l => l.Length > 10)
            .ToList();

        return string.Join(" ", lines);
    }
}
