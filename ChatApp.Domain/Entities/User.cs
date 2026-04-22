namespace ChatApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; 
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    
    public ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();
}