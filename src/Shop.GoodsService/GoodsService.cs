using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.Extensions;
using Shop.Common;
using Shop.IGoods;

namespace Shop.GoodsService
{
    public class GoodsService : IGoodsService
    {
        private IDapper Dapper { get; }
        public GoodsService(IDapper dapper)
        {
            Dapper = dapper;
        }
        public async Task<List<GoodsList>> List(int category)
        {
            return await Task.FromResult(await Dapper.QueryAsync<GoodsList>("select * from goods"));
        }
    }
}
