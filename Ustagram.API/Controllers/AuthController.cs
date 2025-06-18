using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ustagram.Application.Abstractions;
using Ustagram.Domain.DTOs;
using Ustagram.Domain.Model;
using BCrypt.Net;
using System;
using System.Threading.Tasks;
using Ustagram.Application.Services;

namespace Ustagram.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IFileService fileService, JwtService jwtService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _fileService = fileService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromForm] UserDTO request)
        {
            _logger.LogInformation("Signup attempt: Username={Username}, FullName={FullName}", request.Username, request.FullName);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid ModelState for signup: Username={Username}", request.Username);
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Phone))
            {
                _logger.LogWarning("Missing required fields: Username={Username}", request.Username);
                return BadRequest(new { error = "FullName, Username, Password, and Phone are required." });
            }

            var existingUser = await _userService.GetUserByUsername(request.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("Username already exists: {Username}", request.Username);
                return Conflict(new { error = "Username already exists." });
            }

            try
            {
                var userDto = new UserDTO
                {
                    FullName = request.FullName,
                    Password = request.Password,
                    Username = request.Username,
                    Phone = request.Phone,
                    Location = request.Location,
                    PhotoPath = request.PhotoPath,
                    Dob = request.Dob,
                    Status = request.Status,
                    MasterType = request.MasterType,
                    Bio = request.Bio,
                    Experience = request.Experience,
                    TelegramUrl = request.TelegramUrl,
                    InstagramUrl = request.InstagramUrl
                };

                // if (request.Photo != null)
                // {
                //     userDto.PhotoPath = await _fileService.SaveFileAsync(request.Photo, "Photos");
                //     _logger.LogInformation("Photo uploaded for user: {PhotoPath}", userDto.PhotoPath);
                // }

                var result = await _userService.CreateUser(userDto);
                var user = await _userService.GetUserByUsername(userDto.Username);
                var token = _jwtService.GenerateToken(user.Id.ToString());
                _logger.LogInformation("User created: Username={Username}, Id={UserId}", user.Username, user.Id);
                return Ok(new { message = result, token, user });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Signup failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Signup failed for Username={Username}", request.Username);
                return StatusCode(500, new { error = "Registration failed. Please try again later." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            _logger.LogInformation("Login attempt: Username={Username}", request.Username);

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Missing username or password");
                return BadRequest(new { error = "Username and Password are required." });
            }

            var userEntity = await _userService.GetUserEntityByUsername(request.Username);
            if (userEntity == null)
            {
                _logger.LogWarning("User not found: Username={Username}", request.Username);
                return Unauthorized(new { error = "Invalid credentials" });
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, userEntity.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Invalid password for Username={Username}", request.Username);
                return Unauthorized(new { error = "Invalid credentials" });
            }

            var user = await _userService.GetUserByUsername(request.Username);
            var token = _jwtService.GenerateToken(user.Id.ToString());
            _logger.LogInformation("Login successful: Username={Username}, UserId={UserId}", user.Username, user.Id);
            return Ok(new { token, user });
        }
    }

    public class LoginRequest
    {
        [FromForm(Name = "username")]
        public string Username { get; set; }

        [FromForm(Name = "password")]
        public string Password { get; set; }
    }
}