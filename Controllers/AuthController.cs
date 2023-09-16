using backend_app.Model;
using backend_app.Services;
using backend_app.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Context _context;
        private readonly string _secretKey;
        private readonly ConfigurationService _configurationService;


        public AuthController(Context context, IConfiguration configuration, ConfigurationService configurationService)
        {
            _context = context;
            _secretKey = configuration["SecretKey"]; 
            _configurationService = configurationService;
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration(RegisterVM model)
        {

            // Check if the username already exists
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                return BadRequest(new { data = "Username already exists" });
            }

            // Check if the email already exists
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return BadRequest(new { data = "Email already exists" });
            }

            string secretKey = BitConverter.ToString(Enumerable.Range(0, 32).Select(_ => (byte)new Random().Next(256)).ToArray()).Replace("-", "");

            User newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hash(model.Password)
            };

            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            User user = _context.Users.FirstOrDefault(e => e.Username == model.Username);

            if (user == null)
            {
                return BadRequest(new { data = "Invalid username" }); 
            }

            if (user.PasswordHash != hash(model.Password))
            {
                return BadRequest(new { data = "Invalid password" }); 
            }

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        [HttpPost]
        [Route("ValidateToken")]
        public async Task<IActionResult> ValidateToken([FromBody] TokenRequestVM model)
          {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };

                SecurityToken validatedToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(model.Token, tokenValidationParameters, out validatedToken);

                var username = principal.FindFirst(ClaimTypes.Name)?.Value;

                // Retrieve the user's information from the database based on the username
                User user = _context.Users.FirstOrDefault(e => e.Username == username);

                if (user == null)
                {
                    return BadRequest(new { data = "User not found" });
                }

                return Ok(new { user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }


        /*
         *
         * Configuration Methods
         *
         */


        [HttpPost]
        [Route("CreateConfiguration")]
        public async Task<IActionResult> CreateConfiguration (CreateConfigurationVM configuration)
        {
            await _configurationService.Create(configuration);

            return Ok();
        }

        [HttpGet]
        [Route("ListConfigurations/{userId}")]
        public async Task<List<Configuration>> ListConfigurations (Guid userId)
        {
            return await _configurationService.Get(userId);
        }

        /*[HttpGet]
        [Route("GetConfiguration/{id}")]
        public async Task<Configuration> GetConfiguration(Guid id)
        {
            return await _configurationService.Get(id);
        }*/

        [HttpGet]
        [Route("DeleteConfiguration/{id}")]
        public async Task<IActionResult> DeleteConfiguration(Guid id)
        {
            await _configurationService.Remove(id);

            return Ok();
        }


        /*
         *
         * Helpers
         *
         */


        [ApiExplorerSettings(IgnoreApi = true)]
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Username),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private string hash(string password)
        {
            string? hashString = null;

            using (SHA256 sha256 = SHA256.Create())
            {
                hashString = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
            }

            return hashString;
        }

    }
}
