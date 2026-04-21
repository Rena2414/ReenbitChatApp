using Azure;
using Azure.AI.TextAnalytics;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Services;

public class SentimentService : ISentimentService
{
    private readonly TextAnalyticsClient _client;
    private readonly ILogger<SentimentService> _logger;

    public SentimentService(TextAnalyticsClient client, ILogger<SentimentService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<SentimentType> AnalyzeTextAsync(string text, CancellationToken cancellationToken)
    {
        try
        {
            // Call Azure Cognitive Services
            DocumentSentiment documentSentiment = await _client.AnalyzeSentimentAsync(text, cancellationToken: cancellationToken);

            return documentSentiment.Sentiment switch
            {
                TextSentiment.Positive => SentimentType.Positive,
                TextSentiment.Negative => SentimentType.Negative,
                TextSentiment.Neutral => SentimentType.Neutral,
                TextSentiment.Mixed => SentimentType.Neutral, // Grouping mixed as neutral for simplicity
                _ => SentimentType.Neutral
            };
        }
        catch (RequestFailedException ex)
        {
            // Log exceptions at the Infrastructure level
            _logger.LogError(ex, "Azure Cognitive Services failed to analyze sentiment for text. Defaulting to Neutral.");
            return SentimentType.Neutral;
        }
    }
}