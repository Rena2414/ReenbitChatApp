namespace ChatApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}