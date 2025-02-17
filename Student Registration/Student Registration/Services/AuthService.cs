using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Student_Registration.Data;
using Student_Registration.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Student_Registration.Services
{
    public class AuthService
    {
        private readonly StudentDbContext _studentcontext;
        private readonly IConfiguration _config;

        public AuthService(StudentDbContext context, IConfiguration config)
        {
            _studentcontext = context;
            _config = config;
        }

        // Authenticate admin user
        public async Task<string?> Authenticate(string username, string password)
        {
            var admin = await _studentcontext.Admins.FirstOrDefaultAsync(a => a.Username == username);
            if (admin == null || !VerifyPassword(admin.Password, password))
                return null;

            return GenerateJwtToken(admin);
        }

        // Verify password (hash comparison)
        private bool VerifyPassword(string hashedPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); // Verifies hashed password
        }

        // Generate JWT Token
        private string GenerateJwtToken(Admin admin)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Method to hash password during registration
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Get admin by username (used during registration)
        public async Task<Admin?> GetAdminByUsernameAsync(string username)
        {
            return await _studentcontext.Admins.FirstOrDefaultAsync(a => a.Username == username);
        }

        // Register a new admin
        public async Task RegisterAdminAsync(Admin newAdmin)
        {
            await _studentcontext.Admins.AddAsync(newAdmin);
            await _studentcontext.SaveChangesAsync();
        }
    }
}
