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
        /// get all goods by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<List<GoodsList>> GoodsList(int category)
        {
            return await Dapper.QueryAsync<GoodsList>("select * from Goods where CategoryId=@categoryId",
                    new {categoryId = category});
        }

        /// <summary>
        /// get specified goods by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GoodsList> GoodsInfo(int id)
        {
            return await Dapper.QueryFirstOrDefaultAsync<GoodsList>("select * from Goods where id=@id",
                    new { id });
        }

        /// <summary>
        /// get goods by id list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<GoodsList>> GoodsInfos(IEnumerable<int> ids)
        {
            return await Dapper.QueryAsync<GoodsList>("select * from Goods where id in @ids",
                new {ids});
        }
    }
}
