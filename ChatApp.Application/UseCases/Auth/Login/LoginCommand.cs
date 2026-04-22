using System.ComponentModel.DataAnnotations;
using ChatApp.Application.DTOs;
using MediatR;

namespace ChatApp.Application.UseCases.Auth.Login;

public record LoginCommand(
    [Required, MinLength(3), MaxLength(50)] string Username,
    [Required, MinLength(8), MaxLength(100)] string Password
) : IRequest<AuthResponseDto>;