// Controllers/AuthController.cs
using EventTicketingApp.DTOs;
using EventTicketingApp.Models;
using EventTicketingApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, "Customer");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new AuthResponseDto(token, user.Name, user.Email));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new AuthResponseDto(token, user.Name, user.Email!));
        }
    }
}