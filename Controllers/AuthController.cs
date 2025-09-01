using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MajesticEcommerceAPI.Data;
using MajesticEcommerceAPI.Models;
using MajesticEcommerceAPI.DTOs.Auth;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto dto)
{
    if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
    {
        return BadRequest(new { message = "Email already in use." });
    }

   

    var salt = Encoding.UTF8.GetBytes(dto.Email); // Using email as salt
    var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: dto.Password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

    var user = new User
    {
        Email = dto.Email,
        PasswordHash = passwordHash,
        Role = "User"
    };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok(new { message = "User registered successfully." });
}


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            // Verify Password
            var hashedInputPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: dto.Password,
                salt: Encoding.UTF8.GetBytes(dto.Email),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (hashedInputPassword != user.PasswordHash)
                return Unauthorized(new { message = "Invalid email or password." });

            // Create JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)  
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] AdminDto dto)
    {
    if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
    {
        return BadRequest(new { message = "Email already in use." });
    }


    var salt = Encoding.UTF8.GetBytes(dto.Email);
    var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: dto.Password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

        var adminUser = new User
    {
        Email = dto.Email,
        PasswordHash = passwordHash,
        Role = "Admin" // Enforced by backend
    };

    _context.Users.Add(adminUser);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Admin created successfully." });
        }

    }
}   
    
 