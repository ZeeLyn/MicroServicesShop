using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extensions;
using Shop.Common;
using Shop.IGoods;

namespace Shop.GoodsService
{
    /// <summary>
    /// goods category service implementation
    /// </summary>
    public class GoodsCategroryService:IGoodsCategoryService
    {
        private IDapper Dapper { get; }

        public GoodsCategroryService(IDapper dapper)
        {
            Dapper = dapper;
        }
        /// <summary>
        /// get all categories
        /// </summary>
        /// <returns></returns>
        public async Task<List<GoodsCategoryBase>> List()
        {
            return await Dapper.QueryAsync<GoodsCategoryBase>("select * from Category");
        }
    }
}
