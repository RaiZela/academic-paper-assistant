using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public class PdfService
{
    public void SaveTextAsPdf(string text, string outputPath)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var paragraphs = text.Split(Environment.NewLine);
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Header().Text($"Transcript-{DateTime.Now:yyyyMMdd_HHmmss}").FontSize(20).SemiBold().AlignCenter();



                page.Content().Column(column =>
                {
                    foreach (var para in paragraphs)
                        column.Item().Text(para).FontSize(12).LineHeight(1.2f);
                });
            });
        })
        .GeneratePdf(outputPath);
    }
}
