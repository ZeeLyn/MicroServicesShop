using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Common;
using Shop.IGoods;

namespace Shop.GoodsService
{
    public class GoodsService : IGoodsService
    {
        public async Task<List<GoodsList>> List(int category)
        {
            return await Task.FromResult(new List<GoodsList>{
                new GoodsList{
                    Id=1,
                    Title="测试商品1",
                    Price=99
                }
            });
        }
    }
}
