using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Common;
using Shop.IGoods;

namespace Shop.Gateway.Goods.Controllers
{
    /// <summary>
    /// goods controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private IGoodsService GoodsService { get; }

        public GoodsController(IGoodsService goodsService)
        {
            GoodsService = goodsService;
        }

        /// <summary>
        /// get all goods
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("lst/{categoryId:int}"), ProducesResponseType(typeof(List<GoodsList>), 200)]
        public async Task<ActionResult<IEnumerable<string>>> GetLst(int categoryId)
        {
            return Ok(await GoodsService.GoodsList(categoryId));
        }

        /// <summary>
        /// get specified goods by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"),ProducesResponseType(typeof(GoodsList), 200)]
        public async Task<ActionResult<GoodsList>> Get(int id)
        {
            return Ok(await GoodsService.GoodsInfo(id));
        }
    }
}
