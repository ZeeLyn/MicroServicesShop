using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Common;
using Shop.Common.Identity;
using Shop.IIdentity;

namespace Shop.Gateway.Identity.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IIdentityService IdentityService { get; }

        public AccountController(IIdentityService identityService)
        {
            IdentityService = identityService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterView registerView)
        {
            var (succeed, errorMessage) = await IdentityService.Register(registerView);
            if (succeed)
                return Ok();
            return BadRequest(errorMessage);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginView login)
        {
            Console.WriteLine(login.Email);
            var (succeed, errorMessage, result) = await IdentityService.Login(login);
            if (succeed)
                return Ok(result);
            return BadRequest(errorMessage);
        }
    }
}
