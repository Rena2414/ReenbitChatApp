using ChatApp.Application.DTOs;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using MediatR;

namespace ChatApp.Application.UseCases.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();

        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken)
                   ?? throw new UnauthorizedException("Invalid username or password.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        var token = _jwtProvider.Generate(user);
        return new AuthResponseDto(user.Id, user.Username, token);
    }
}