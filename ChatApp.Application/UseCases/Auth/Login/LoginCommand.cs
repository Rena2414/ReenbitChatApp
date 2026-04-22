using ChatApp.Application.DTOs;
using MediatR;

namespace ChatApp.Application.UseCases.Auth.Login;

public record LoginCommand(string Username, string Password) : IRequest<AuthResponseDto>;