using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Shop.Common.Basket;
using Shop.IBasket;
using Shop.IGoods;

namespace Shop.BasketService
{
    public class BasketService : IBasketService
    {
        private IGoodsService GoodsService { get; }

        private ICapPublisher CapBus { get; }

        public BasketService(IGoodsService goodsService, ICapPublisher capPublisher)
        {
            GoodsService = goodsService;
            CapBus = capPublisher;
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
            var list = (await RedisHelper.HGetAllAsync<int>(userid.ToString())).Select(p => new { Id = int.Parse(p.Key), Count = p.Value }).ToList();
            var goods = await GoodsService.GoodsInfos(list.Select(p => p.Id).ToList());
            return list.Select(p => new Basket
            {
                GoodsId = p.Id,
                Count = p.Count,
                Price = goods.FirstOrDefault(i => i.Id == p.Id)?.Price ?? 0,
                Title = goods.FirstOrDefault(i => i.Id == p.Id)?.Title,
                Pic = goods.FirstOrDefault(i => i.Id == p.Id)?.Pic
            }).ToList();
        }

        public async Task<int> Count(int userid)
        {
            return (await RedisHelper.HGetAllAsync<int>(userid.ToString())).Sum(p => p.Value);
        }

        #region check out

        public async Task<bool> CheckOut(int userid, List<int> goodsId)
        {
            var basket = (await RedisHelper.HGetAllAsync<int>(userid.ToString())).Select(p => new BasketBase
            {
                GoodsId = int.Parse(p.Key),
                Count = p.Value
            }).Where(p => goodsId.Any(i => i == p.GoodsId)).ToList();

            await CapBus.PublishAsync("route.basket.checkout", new CheckOut
            {
                UserId = userid,
                Basket = basket
            }, "CheckOutCallback");
            return true;
        }

        /// <summary>
        /// Checkout call back
        /// </summary>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        public async Task CheckOutCallback(CheckOut checkOut)
        {
            await RedisHelper.HDelAsync(checkOut.UserId.ToString(), checkOut.Basket.Select(p => p.GoodsId.ToString()).ToArray());
        }
        #endregion
    }
}
