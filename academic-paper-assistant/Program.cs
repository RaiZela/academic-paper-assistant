using academic_paper_assistant;
using Azure;
using Azure.AI.TextAnalytics;
using Azure.AI.Translation.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AcademicPaperService>();
builder.Services.AddScoped<TranslatorService>();
builder.Services.AddScoped<SummarizeTranslation>();

builder.Services.AddSingleton(new TextAnalyticsClient(
    new Uri(builder.Configuration["AzureAI:LANGUAGE_ENDPOINT"]!),
    new AzureKeyCredential(builder.Configuration["AzureAI:LANGUAGE_KEY"]!)
));
builder.Services.AddSingleton(new TextTranslationClient(
    new Uri(builder.Configuration["AzureAI:LANGUAGE_ENDPOINT"]!)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/academic-paper", (string path, SummarizeTranslation service) =>
{
    var result = service.GetSummaryAndTranslation(path);
    return Results.Ok(new { Summarize = result });
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
