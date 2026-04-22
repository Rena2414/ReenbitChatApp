using ChatApp.Application.DTOs;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.UseCases.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();

        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("Username is required.");

        var existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (existingUser != null)
            throw new ValidationException("Username already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken); 
        
        var token = _jwtProvider.Generate(user);
        return new AuthResponseDto(user.Id, user.Username, token);
    }
}