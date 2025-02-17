using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Registration.Models;
using Student_Registration.Services;
using System.Threading.Tasks;

namespace Student_Registration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Register Admin
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto registerDto)
        {
            // Check if admin already exists
            var existingAdmin = await _authService.GetAdminByUsernameAsync(registerDto.Username);
            if (existingAdmin != null)
                return Conflict(new { message = "Admin already exists." });

            // Hash password
            var hashedPassword = _authService.HashPassword(registerDto.Password);

            // Create a new admin instance
            var newAdmin = new Admin
            {
                Username = registerDto.Username,
                Password = hashedPassword
            };

            // Register admin using the service
            await _authService.RegisterAdminAsync(newAdmin);

            // Return a successful response with the created admin
            return CreatedAtAction(nameof(RegisterAdmin), new { username = newAdmin.Username }, newAdmin);
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.Authenticate(loginDto.Username, loginDto.Password);
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }
    }

    // DTO for Register Admin
    public class RegisterAdminDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // DTO for Login
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
