using AtomicHabits.Models.DTO;
using AtomicHabits.Service;
using AtomicHabits.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtomicHabits.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            var res = await _authService.RegisterAsync(dto, cancellationToken);
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var res = await _authService.LoginAsync(dto, cancellationToken);
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto, CancellationToken cancellationToken)
        {
            var res = await _authService.ForgotPasswordAsync(dto, cancellationToken);
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto, CancellationToken cancellationToken)
        {
            var res = await _authService.ResetPasswordAsync(dto, cancellationToken);
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
        {
            var res = await _authService.RefreshTokenAsync(dto, cancellationToken);
            return StatusCode((int)res.StatusCode, res);
        }

    }
}
