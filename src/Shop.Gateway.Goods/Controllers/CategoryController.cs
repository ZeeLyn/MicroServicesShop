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
    /// category controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IGoodsCategoryService CategoryService { get; }

        public CategoryController(IGoodsCategoryService categoryService)
        {
            CategoryService = categoryService;
        }

        /// <summary>
        /// get all categroy
        /// </summary>
        /// <returns></returns>
        [HttpGet("lst"), ProducesResponseType(typeof(List<GoodsList>), 200)]
        public async Task<ActionResult<IEnumerable<string>>> GetLst()
        {
            return Ok(await CategoryService.List());
        }
    }
}
