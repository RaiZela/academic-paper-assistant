namespace academic_paper_assistant.Models;

public class ExtractedSummary
{
    public string ErrorCode { get; set; }
    public string Message { get; set; }
    public string ExtractedDescription { get; set; }
    public List<string> Sentences { get; set; } = new List<string>();
}
