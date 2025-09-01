using academic_paper_assistant.Services;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class SpeechToTextService
{
    private readonly SpeechConfig _speechConfig;
    private SpeechRecognizer speechRecognizer;
    private List<string> AllRecognizedText = new List<string>();
    public SpeechToTextService(SpeechConfig speechConfig)
    {
        _speechConfig = speechConfig;
    }

    public async void RecognizeSpeechAsync(string language)
    {
        var matchedKey = SpeechLocales.Locales.Keys
    .FirstOrDefault(k => k.Contains(language, StringComparison.OrdinalIgnoreCase));

        if (matchedKey == null)
        {
            throw new ArgumentException($"Language '{language}' is not supported.");
        }

        _speechConfig.SpeechRecognitionLanguage = SpeechLocales.Locales[matchedKey];
        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        speechRecognizer = new SpeechRecognizer(_speechConfig, audioConfig);
        Console.WriteLine("Speak into your microphone. Press Enter to stop...");

        speechRecognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"Recognized: {e.Result.Text}");
                AllRecognizedText.Add(e.Result.Text);
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine("No speech recognized.");
            }
        };

        await speechRecognizer.StartContinuousRecognitionAsync();
    }

    public async Task<string> StopSpeechAsync()
    {
        if (speechRecognizer == null)
            throw new InvalidOperationException("Recognizer not started.");

        await speechRecognizer.StopContinuousRecognitionAsync();
        speechRecognizer.Dispose();
        speechRecognizer = null;

        string finalTranscript = string.Join(Environment.NewLine, AllRecognizedText);

        string folderPath = Path.Combine(Environment.CurrentDirectory, "Transcripts");
        Directory.CreateDirectory(folderPath);

        string fileName = $"transcript-{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        string fullPath = Path.Combine(folderPath, fileName);

        var pdfService = new PdfService();
        pdfService.SaveTextAsPdf(finalTranscript, fullPath);

        AllRecognizedText = null;
        return fullPath;
    }

    static string OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                return $"RECOGNIZED: Text={speechRecognitionResult.Text}";
            case ResultReason.NoMatch:
                return $"NOMATCH: Speech could not be recognized.";
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                var textReturned = $"CANCELED: Reason={cancellation.Reason}";

                if (cancellation.Reason == CancellationReason.Error)
                {
                    textReturned = $"\n CANCELED: ErrorCode={cancellation.ErrorCode}";
                    textReturned = $"\n CANCELED: ErrorDetails={cancellation.ErrorDetails}";
                    textReturned = $"\n CANCELED: Did you set the speech resource key and endpoint values?";
                }
                return textReturned;
            default:
                return string.Empty;
        }
    }



}