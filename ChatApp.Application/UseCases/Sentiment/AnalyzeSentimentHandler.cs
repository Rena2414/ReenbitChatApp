using ChatApp.Application.DTOs;
using ChatApp.Application.Interfaces.Services;
using MediatR;

namespace ChatApp.Application.UseCases.Sentiment;

public record AnalyzeSentimentQuery(string Text) : IRequest<SentimentResultDto>;

public class AnalyzeSentimentHandler : IRequestHandler<AnalyzeSentimentQuery, SentimentResultDto>
{
    private readonly ISentimentService _sentimentService;

    public AnalyzeSentimentHandler(ISentimentService sentimentService)
    {
        _sentimentService = sentimentService;
    }

    public async Task<SentimentResultDto> Handle(AnalyzeSentimentQuery request, CancellationToken cancellationToken)
    {
        var sentiment = await _sentimentService.AnalyzeTextAsync(request.Text, cancellationToken);
        return new SentimentResultDto(sentiment.ToString());
    }
}