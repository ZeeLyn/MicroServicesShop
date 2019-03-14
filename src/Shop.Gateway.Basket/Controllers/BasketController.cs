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
    }
}
