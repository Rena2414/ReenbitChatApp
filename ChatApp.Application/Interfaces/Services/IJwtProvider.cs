using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Services;

public interface IJwtProvider
{
    string Generate(User user);
}