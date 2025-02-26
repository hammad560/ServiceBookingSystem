using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceBookingSystemAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServiceBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly ApplicationDbContext Context;
        public readonly IConfiguration Configuration;
        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            this.Context = dbContext;
            this.Configuration = configuration;
        }


        #region Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(User userDto)
        {
            try
            {
                // Check if user already exists
                var userExists = await Context.Users.AnyAsync(u => u.Email == userDto.Email);
                if (userExists)
                {
                    return BadRequest("User already exists!");
                }
                // new user
                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Role = userDto.Role
                };

                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, userDto.Password);

                await Context.Users.AddAsync(user);
                await Context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully!" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }
        #endregion

        #region login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(User userDto)
        {
            try
            {
                // Check if user exists
                var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }
                // Verify password
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, userDto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return Unauthorized("Invalid password.");
                }
                // Generate JWT token
                var token = GenerateJwtToken(user);

                //  Return token
                return Ok(new { Token = token });
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }
        #endregion

        #region GenerateJwtToken
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("UserId", user.Id.ToString()),
            new Claim("UserEmail", user.Email),
            new Claim("Role", user.Role)
             };

            var token = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
