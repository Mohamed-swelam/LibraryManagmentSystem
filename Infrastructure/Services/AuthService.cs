using Core.DTOs;
using Core.DTOs.AuthDTO;
using Core.Entites;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUserRepository userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager
            , IOptions<JwtSettings> options, SignInManager<ApplicationUser> signInManager, IUserRepository userRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.userRepository = userRepository;
            _jwtSettings = options.Value;
        }

        public async Task<GeneralResponse> LoginAsync(LoginDTO loginDTO)
        {
            var userfromDb = await userManager.FindByEmailAsync(loginDTO.Email);
            if (userfromDb == null)
            {
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = "No Account With This Email.."
                };
            }
            var result = await userManager.CheckPasswordAsync(userfromDb, loginDTO.Password);
            if (!result)
            {
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = "Password incorrect.."
                };
            }

            string finalToken = await JwtTokenGenerator(userfromDb);
            string refreshToken = RefreshTokenGenerator.GenerateRefreshToken();
            userfromDb.RefreshTokens?.Add(new RefreshToken
            {
                Token = refreshToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(3)
            });

            await userRepository.SaveChangesAsync();

            return new GeneralResponse
            {
                isSuccess = true,
                Data = new
                {
                    Token = finalToken,
                    RefreshToken = refreshToken,
                    expiration = DateTime.Now.AddHours(3)
                }
            };
        }

        public async Task<GeneralResponse> LogoutAsync(string userId)
        {
            var user = await userRepository.GetUserById(userId);

            if (user == null)
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = "User is not authenticated"
                };

            if (user?.RefreshTokens != null)
            {
                foreach (var token in user.RefreshTokens)
                {
                    token.Revoked = DateTime.UtcNow;
                }
            }
            await userRepository.SaveChangesAsync();
            await signInManager.SignOutAsync();

            return new GeneralResponse
            {
                isSuccess = true,
                Data = "LogOut Succesfully"
            };
        }

        public async Task<GeneralResponse> RefreshTokenAsync(TokenRequest tokenRequest)
        {
            var user = await userRepository.GetUserByRefreshtoken(tokenRequest.RefreshToken);

            if (user == null || user.RefreshTokens is null)
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = "User is not authenticated"
                };

            var refreshToken = user.RefreshTokens
                    .Single(e => e.Token == tokenRequest.RefreshToken);

            if (!refreshToken.IsActive)
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = "refreshtoken has expired."
                };

            refreshToken.Revoked = DateTime.UtcNow;
            var newToken = await JwtTokenGenerator(user);
            var newRefreshToken = RefreshTokenGenerator.GenerateRefreshToken();
            user.RefreshTokens?.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(3)
            });

            await userRepository.SaveChangesAsync();

            return new GeneralResponse
            {
                isSuccess = true,
                Data = new
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    expiration = DateTime.Now.AddHours(3)
                }
            };
        }

        public async Task<GeneralResponse> RegisterAsync(RegisterDTO registerDTO)
        {
            var userfromDb = await userManager.FindByEmailAsync(registerDTO.Email);
            if (userfromDb != null)
            {
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = "There is an Account with this Email"
                };
            }

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
            {
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = result.Errors.Select(e => e.Description).ToList()
                };
            }

            var roolExist = await roleManager.RoleExistsAsync("Member");
            if (roolExist)
                await userManager.AddToRoleAsync(user, "Member");
            else
            {
                return new GeneralResponse
                {
                    isSuccess = false,
                    Data = "Role Doesn't Exist"
                };
            }

            return new GeneralResponse
            {
                isSuccess = true,
                Data = "Registration Succesfully.."
            };
        }


        private async Task<string> JwtTokenGenerator(ApplicationUser user)
        {
            List<Claim> userclaims = new();
            userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userclaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userclaims.Add(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));


            var Roles = await userManager.GetRolesAsync(user);
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
