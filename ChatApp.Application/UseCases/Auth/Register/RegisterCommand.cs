using ChatApp.Application.DTOs;
using MediatR;

namespace ChatApp.Application.UseCases.Auth.Register;

public record RegisterCommand(string Username, string Password) : IRequest<AuthResponseDto>;