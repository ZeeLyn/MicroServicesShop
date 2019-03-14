using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Common.Basket;
using Shop.IGoods;

namespace Shop.BasketService
{
    public class BasketService : IBasket.IBasketService
    {
        private IGoodsService GoodsService { get; }

        public BasketService(IGoodsService goodsService)
        {
            GoodsService = goodsService;
        }

        public async Task Add(int userid, int goodsId, int count)
        {
            await RedisHelper.HIncrByAsync(userid.ToString(), goodsId.ToString(), count);
        }

        public async Task Remove(int userid, int goodsId)
        {
            await RedisHelper.HDelAsync(userid.ToString(), goodsId.ToString());
        }

        public async Task<List<Basket>> Get(int userid)
        {
            var list = await RedisHelper.HGetAllAsync<int>(userid.ToString());
            return list.Select(p => new Basket
            {
                GoodsId = int.Parse(p.Key),
                Count = p.Value
            }).ToList();
        }

        public async Task<int> Count(int userid)
        {
            return (int)await RedisHelper.HLenAsync(userid.ToString());
        }
    }
}
