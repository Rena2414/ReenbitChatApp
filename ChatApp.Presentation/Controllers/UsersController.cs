using ChatApp.Application.DTOs;
using ChatApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository) => _userRepository = userRepository;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        // In a real app, you might use a Query/Handler here, 
        // but for a simple user list, direct repo access in the controller is often fine.
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var userDtos = users.Select(u => new UserDto(u.Id, u.Username));
        return Ok(userDtos);
    }
}