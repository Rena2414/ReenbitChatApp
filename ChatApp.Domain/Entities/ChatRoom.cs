namespace ChatApp.Domain.Entities;

public class ChatRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}