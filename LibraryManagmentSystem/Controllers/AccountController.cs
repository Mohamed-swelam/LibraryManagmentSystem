using Core.DTOs;
using Core.DTOs.AuthDTO;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService service;

        public AccountController(IAuthService service)
        {
            this.service = service;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse
                {
                    isSuccess = false,
                    Data = ModelState
                });

            var response = await service.RegisterAsync(registerDTO);
            if (response.isSuccess)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse
                {
                    isSuccess = false,
                    Data = ModelState
                });

            var response = await service.LoginAsync(loginDTO);
            if (response.isSuccess)
                return Ok(response);
            return BadRequest(response);
        }

        [Authorize]
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(
                    new GeneralResponse
                    {
                        isSuccess = false,
                        Data = "User is not authenticated"
                    });

            var response = await service.LogoutAsync(userId);

            if (response.isSuccess)
                return Ok(response);
            return Unauthorized(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
        {
            var response = await service.RefreshTokenAsync(tokenRequest);

            if (response.isSuccess)
                return Ok(response);
            return Unauthorized(response);
        }

    }
}
