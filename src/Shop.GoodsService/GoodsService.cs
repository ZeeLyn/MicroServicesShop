using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.Extensions;
using Shop.Common;
using Shop.IGoods;

namespace Shop.GoodsService
{
    /// <summary>
    /// goods service implementation
    /// </summary>
    public class GoodsService : IGoodsService
    {
        private IDapper Dapper { get; }
        public GoodsService(IDapper dapper)
        {
            Dapper = dapper;
        }
        /// <summary>
        /// get all goods by s
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<List<GoodsList>> GoodsList(int category)
        {
            return await Task.FromResult(
                await Dapper.QueryAsync<GoodsList>("select * from goods where CategoryId=@categoryId",
                    new {categoryId = category}));
        }
    }
}
