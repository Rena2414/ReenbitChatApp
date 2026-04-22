using System.ComponentModel.DataAnnotations;
using ChatApp.Application.DTOs;
using MediatR;

namespace ChatApp.Application.UseCases.Auth.Register;

public record RegisterCommand(
    [Required, MinLength(3), MaxLength(50)] string Username,
    [Required, MinLength(8), MaxLength(100)] string Password
) : IRequest<AuthResponseDto>;