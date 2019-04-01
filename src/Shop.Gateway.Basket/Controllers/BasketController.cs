using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JWT.Extension;
using Microsoft.AspNetCore.Mvc;
using Shop.Common.Basket;
using Shop.IBasket;

namespace Shop.Gateway.Basket.Controllers
{
    [Route("api/basket")]
    [ApiController]
    [Permission]
    public class BasketController : ControllerBase
    {
        private IBasketService BasketService { get; }

        public BasketController(IBasketService basketService)
        {
            BasketService = basketService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody]BasketBase data)
        {
            if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userid))
                return Unauthorized();
            await BasketService.Add(userid, data.GoodsId, data.Count);
            return Ok();
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userid))
                return Unauthorized();
            return Ok(await BasketService.Count(userid));
        }

        [HttpGet("detail")]
        public async Task<IActionResult> List()
        {
            try
            {
                if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userid))
                    return Unauthorized();
                return Ok(await BasketService.Get(userid));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("remove/{id:int}")]
        public async Task<IActionResult> Remove(int id)
        {
            if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userid))
                return Unauthorized();
            await BasketService.Remove(userid, id);
            return Ok();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody]List<int> goods)
        {
            if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userid))
                return Unauthorized();
            var result = await BasketService.CheckOut(userid, goods);
            if (result.Succeed)
                return Ok(result.OrderCode);
            return BadRequest(result.ErrorMessage);
        }
    }
}
