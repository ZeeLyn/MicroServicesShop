using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JWT.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Common.Order;
using Shop.IOrder;

namespace Shop.Gateway.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Permission]
    public class OrderController : ControllerBase
    {
        private IOrderService OrderService { get; }

        public OrderController(IOrderService orderService)
        {
            OrderService = orderService;
        }
        /// <summary>
        /// get all orders from user id.
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetAllOrder()
        {
            if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userId))
                return Unauthorized();
            var result = await OrderService.GetAllOrder(userId);
            return Ok(result);
        }
        /// <summary>
        /// get specified order by order code.
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        [HttpGet("detail/{orderCode}")]
        public async Task<IActionResult> GetOrder(string orderCode)
        {
            if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userId))
                return Unauthorized();
            var result = await OrderService.GetOrder(userId, orderCode);
            if (result.Succeed)
                return Ok(result.Order);
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// cancel order.
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        [HttpGet("cancel/{orderCode}")]
        public async Task<IActionResult> Cancel(string orderCode)
        {
            if (!int.TryParse(User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Sid)?.Value, out var userId))
                return Unauthorized();
            var order = await OrderService.GetOrder(userId, orderCode);
            if (!order.Succeed)
                return BadRequest(order.ErrorMessage);
            var result = await OrderService.CancelOrder(userId, orderCode);
            if (!result.Result)
                return Ok(result.Error);
            return Ok(result.Result);
        }
    }
}