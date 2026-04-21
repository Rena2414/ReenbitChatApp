using ChatApp.Domain.Enums;

namespace ChatApp.Application.Interfaces.Services;

public interface ISentimentService
{
    Task<SentimentType> AnalyzeTextAsync(string text, CancellationToken cancellationToken);
}