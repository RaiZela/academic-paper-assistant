# Academic Paper Assistant & Speech-to-Text PDF Tool

A .NET project that allows you to:
- Convert speech to text in multiple languages and save as PDF transcripts.
- Summarize PDF content using Azure Text Analytics.
- Translate summaries into another language with Azure Translator.

This project is ideal for researchers, students, or anyone who wants to quickly summarize and translate PDF documents or record speech and save transcripts.

## Features
### 1. Speech-to-Text

- Continuous speech recognition with Microsoft Azure Cognitive Services.
- Real-time display of recognized speech.
- Saves recognized text to a PDF file using QuestPDF.
- Supports multiple languages via SpeechLocales.

### 2. PDF Summarization

- Extracts text from PDFs.
- Uses Azure Text Analytics to generate extractive summaries.
- Handles multiple sentences with proper error handling.

### 3. Translation

- Translates summarized sentences into a specified language.
- Returns both original and translated sentences for comparison.

## Prerequisites

- .NET 6 or later
- Azure Cognitive Services Key and Endpoint for:
     1)Speech-to-Text
     2)Text Analytics (summarization)
     3)Translator

- NuGet Packages:
```bash
dotnet add package Microsoft.CognitiveServices.Speech
dotnet add package Azure.AI.TextAnalytics
dotnet add package QuestPDF
```

## Usage
### 1. Speech-to-Text

```csharp
var speechConfig = SpeechConfig.FromSubscription("YOUR_KEY", "YOUR_REGION");
var speechService = new SpeechToTextService(speechConfig);

// Start recognition
await speechService.RecognizeSpeechAsync("en-US");

// Stop recognition and save PDF
string pdfPath = await speechService.StopSpeechAsync();
Console.WriteLine($"Transcript saved at: {pdfPath}");
```
### 2. Summarize & Translate PDF

```csharp
var academicService = new AcademicPaperService(textAnalyticsClient);
var translatorService = new TranslatorService(translatorClient);

var summarizeTranslation = new SummarizeTranslation(academicService, translatorService);

var results = await summarizeTranslation.GetSummaryAndTranslation("path-to-pdf.pdf", "es"); // Spanish

foreach (var r in results)
{
    Console.WriteLine($"Original: {r.OriginalSentence}");
    Console.WriteLine($"Translated: {r.TranslatedSentence}");
}
```

## Folder Structure

```graphql
/Transcripts           # Saved PDFs from speech recognition
/Services              # SpeechToTextService, AcademicPaperService, TranslatorService
/Models                # SummaryTranslationResult, ExtractedSummary
/Program.cs            # Entry point
```

## Notes

- QuestPDF requires a license type even for free usage:
```csharp
QuestPDF.Settings.License = LicenseType.Community;
```

- Each speech session generates a separate PDF with a timestamped filename:

```bash
transcript-20250901_081530.pdf
```

- Azure services must be configured with valid keys and endpoints.

## Future Enhancements

- Add multi-language speech-to-text support dynamically.
- Combine multiple PDF summaries into a single report.
- Add GUI for easy file selection and language choice.
