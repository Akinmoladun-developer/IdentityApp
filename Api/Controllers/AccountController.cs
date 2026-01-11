using Api.Data;
using Api.DTOs;
using Api.DTOs.Account;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Api.Controllers
{

    // The route of our controller becomes the url of our application
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(JWTService jwtService,
            SignInManager<User> signInManager, // Responsible to sign User In
            UserManager<User> userManager) // For creating the User
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")] // Take Note: We used HttpPost since we are posting a model to this method 
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return BadRequest("Invalid UserName and Password");

            if (user.EmailConfirmed == false) return Unauthorized("Confirm your Email!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid user name or password");

            return CreateApplicationUserDto(user);
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterDto model) 
        {
            if (await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest($"An existing account is using{model.Email}, email address. Please try with another email address");

            }

            var userToAdd = new User
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // return ok("Your account has been created, You can now login");
            return Ok(new JsonResult(new { tite = "Account Created", message = "You account has been created you can now login"}));
        }

        #region Private Helper Methods
        private UserDto CreateApplicationUserDto(User user)
        {
             return new UserDto
             {
                  FirstName = user.FirstName,
                  LastName = user.LastName,
                  JWT = _jwtService.CreateJWT(user),
              };
        }
        #endregion

        private async Task<bool> CheckEmailExistsAsync(string email) 
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }

}

