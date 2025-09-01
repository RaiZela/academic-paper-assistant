using academic_paper_assistant.Services;
using Azure;
using Azure.AI.TextAnalytics;
using Azure.AI.Translation.Text;
using Microsoft.CognitiveServices.Speech;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AcademicPaperService>();
builder.Services.AddScoped<TranslatorService>();
builder.Services.AddScoped<SummarizeTranslation>();
builder.Services.AddSingleton<SpeechToTextService>();

builder.Services.AddSingleton(new TextAnalyticsClient(
    new Uri(builder.Configuration["AzureAI:LANGUAGE_ENDPOINT"]!),
    new AzureKeyCredential(builder.Configuration["AzureAI:LANGUAGE_KEY"]!)
));

builder.Services.AddSingleton(serviceProvider =>
{
    var config = SpeechConfig.FromEndpoint(
        new Uri(builder.Configuration["AzureSpeech:ENDPOINT"]!),
        builder.Configuration["AzureSpeech:KEY"]!
    );
    return config;
});


builder.Services.AddSingleton(new TextTranslationClient(new AzureKeyCredential(builder.Configuration["AzureTranslator:KEY"]!),
    new Uri(builder.Configuration["AzureTranslator:ENDPOINT"]!), builder.Configuration["AzureTranslator:REGION"]!));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/summarize-and-translate", (string path, string language, SummarizeTranslation service) =>
{
    var result = service.GetSummaryAndTranslation(path, language);
    return Results.Ok(new { Summarize = result });
})
.WithOpenApi();

app.MapGet("/speech-to-text", (string language, SpeechToTextService service) =>
{
    service.RecognizeSpeechAsync(language);
    return Results.Ok();
})
.WithOpenApi();

app.MapGet("/stop-speech-to-text", (SpeechToTextService service) =>
{
    var result = service.StopSpeechAsync();
    return Results.Ok(new { Text = result });
})
.WithOpenApi();

app.Run();
