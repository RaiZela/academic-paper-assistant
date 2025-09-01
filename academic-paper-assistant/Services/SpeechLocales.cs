namespace academic_paper_assistant.Services;

public static class SpeechLocales
{
    public static readonly Dictionary<string, string> Locales = new()
    {
        { "English (United States)", "en-US" },
        { "English (United Kingdom)", "en-GB" },
        { "French (France)", "fr-FR" },
        { "French (Canada)", "fr-CA" },
        { "German (Germany)", "de-DE" },
        { "Italian (Italy)", "it-IT" },
        { "Spanish (Spain)", "es-ES" },
        { "Spanish (Mexico)", "es-MX" },
        { "Portuguese (Brazil)", "pt-BR" },
        { "Portuguese (Portugal)", "pt-PT" },
        { "Russian (Russia)", "ru-RU" },
        { "Arabic (Egypt)", "ar-EG" },
        { "Chinese (Mandarin, Simplified)", "zh-CN" },
        { "Japanese (Japan)", "ja-JP" },
        { "Korean (Korea)", "ko-KR" }
    };
}
