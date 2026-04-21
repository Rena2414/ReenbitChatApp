using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public SentimentType Sentiment { get; set; } = SentimentType.Neutral; 
    
    public Guid ChatRoomId { get; set; }
    public ChatRoom? ChatRoom { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }
}