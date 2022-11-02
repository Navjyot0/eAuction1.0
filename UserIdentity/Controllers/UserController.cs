using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserIdentity.JWT;
using UserIdentity.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserIdentity.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserManager<ApplicationUser> _userManager;
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;

        public UserController(UserManager<ApplicationUser> userManager, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            this._userManager = userManager;
            this.jwtAuthenticationManager = jwtAuthenticationManager;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<UserController>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Post(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    UserName = user.Email,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(applicationUser, user.Password);
                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status201Created);
                 return StatusCode(StatusCodes.Status400BadRequest, result.Errors);
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserCred userCred)
        {
            var token = jwtAuthenticationManager.Authenticate(userCred.Name, userCred.Password);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
    }
}
