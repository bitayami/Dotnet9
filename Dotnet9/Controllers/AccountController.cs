using Dotnet9.Models;
using Dotnet9.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AccountController(UserManager<AppUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto) { 
            AppUser user = new AppUser()
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) 
            {
                return BadRequest(result);
            }

            //await _userManager.AddToRoleAsync(user, "User");

            return Ok(result);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto) {
           var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password)) {
                var userRoles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                //claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                foreach( var role in userRoles)
                    claims.Add(new Claim(ClaimTypes.Role, role));


                var loginKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["jwt:Key"]!));

                var tokenhandler = new JwtSecurityToken(
                    issuer: _config["jwt:Issuer"],
                    audience: _config["jwt:Audience"],
                    claims: claims,
                    signingCredentials: new SigningCredentials(
                        loginKey, SecurityAlgorithms.HmacSha256Signature
                        ),
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["jwt:ExpiryMinutes"]))
                    );

                //var tokenDescriptor = new SecurityTokenDescriptor
                //{
                //    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["jwt:ExpiryMinutes"])),
                //    SigningCredentials = new SigningCredentials(
                //        loginKey, SecurityAlgorithms.HmacSha256Signature
                //        ),
                //    Claims = (IDictionary<string, object>)claims
                //};

                //var tokenHandler = new JwtSecurityTokenHandler();
                //var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                //var token = tokenHandler.WriteToken(securityToken);

                var token = new JwtSecurityTokenHandler().WriteToken(tokenhandler);

                return Ok(token);
            }
            else
            {
                return BadRequest(dto);
            }
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole ([FromBody] string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));

                if (result.Succeeded)
                {
                    return Ok(new { message = "Role added successfully" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest("Role already exist");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserName);
            if (user == null)
            {
                return BadRequest("user not found");
            }
            var isRole = await _roleManager.RoleExistsAsync(model.Role);
            if (!isRole)
            {
                return BadRequest("role not found");
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned Successfully" });
            }
            return BadRequest(result.Errors);
        }
    }
}
