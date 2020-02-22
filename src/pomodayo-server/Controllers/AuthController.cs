using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pomodayo.server.Models;
using pomodayo.server.Services.Contracts;

namespace pomodayo.server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IDataStoreService _dataStore;

        public AuthController(IDataStoreService dataStore)
        {
            _dataStore = dataStore;
        }

        [HttpGet("signed-in")]
        [Authorize]
        public IActionResult IsSignedIn()
        {
            return Ok();
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInViewModel signIn)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity();

            var user = await _dataStore.GetUserAsync(signIn.UserName).ConfigureAwait(false);
            if (user == null)
                return Unauthorized();

            if (user.Password != signIn.Password)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Ok();
        }

        [HttpPost("sign-out")]
        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUser(CreateUserViewModel user)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity();

            var createdUser = await _dataStore.CreateUserAsync(user.UserName, user.Password).ConfigureAwait(false);
            if (createdUser == null)
                return Conflict();

            return Ok(createdUser);
        }
    }
}
