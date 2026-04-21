namespace ChatApp.Application.DTOs;

public record MessageDto(
    Guid Id, 
    string Content, 
    string Username, 
    DateTime Timestamp, 
    string Sentiment);