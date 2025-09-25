using Core.DTOs.AuthDTO;
using Core.Entites;
using Core.Helpers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JwtSettings _jwtSettings;

        public AccountController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager
            , UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager
            , IOptions<JwtSettings> options)
        {
            this.context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._jwtSettings = options.Value;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userfromDb = await userManager.FindByEmailAsync(registerDTO.Email);
            if (userfromDb != null)
                return BadRequest("There is an Account with this Email");

            var user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Address = registerDTO.Address,
                PhoneNumber = registerDTO.PhoneNumber,
                DateOfBirth = registerDTO.DateofBirth,
                JoinedDate = DateTime.UtcNow,
            };
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description).ToList());

            var roolExist = await roleManager.RoleExistsAsync("Member");
            if (roolExist)
                await userManager.AddToRoleAsync(user, "Member");
            else
                return BadRequest("Role Doesn't Exist");

            return Ok("Registration Succesfully..");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userfromDb = await userManager.FindByEmailAsync(loginDTO.Email);
            if (userfromDb == null)
                return BadRequest("No Account With This Email..");

            var result = await userManager.CheckPasswordAsync(userfromDb, loginDTO.Password);
            if (!result)
                return BadRequest("Password incorrect..");

            List<Claim> userclaims = new();
            userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userclaims.Add(new Claim(ClaimTypes.NameIdentifier, userfromDb.Id));
            userclaims.Add(new Claim(ClaimTypes.Email, userfromDb.Email ?? string.Empty));


            var Roles = await userManager.GetRolesAsync(userfromDb);
            foreach (var Role in Roles)
            {
                userclaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecritKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: _jwtSettings.IssuerIP,
               audience: _jwtSettings.AudienceIP,
               expires: DateTime.UtcNow.AddMinutes(30),
               claims: userclaims,
               signingCredentials: credentials
            );

            string finalToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Token = finalToken,
                expiration = DateTime.Now.AddHours(3)
            });
        }

        [Authorize]
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("User is not authenticated");

            await signInManager.SignOutAsync();

            return Ok("LogOut Succesfully");
        }



    }
}
