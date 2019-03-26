using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Shop.Common.Basket;
using Shop.Common.Order;
using Shop.IBasket;
using Shop.IGoods;
using Shop.IOrder;

namespace Shop.BasketService
{
    public class BasketService : IBasketService, ICapSubscribe
    {
        private IGoodsService GoodsService { get; }

        private IOrderService OrderService { get; }

        private ICapPublisher CapBus { get; }
        private ILogger Logger { get; }

        public BasketService(IGoodsService goodsService, ICapPublisher capPublisher, ILogger<BasketService> logger, IOrderService orderService)
        {
            GoodsService = goodsService;
            CapBus = capPublisher;
            Logger = logger;
            OrderService = orderService;
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

        public async Task<(bool Succeed, string OrderCode, string ErrorMessage)> CheckOut(int userid, List<int> goodsId)
        {
            var basket = (await Get(userid)).Where(p => goodsId.Any(i => i == p.GoodsId)).Select(p => new GoodsInfo
            {
                GoodsId = p.GoodsId,
                Count = p.Count,
                Price = p.Price
            }).ToList();

            var result = await OrderService.Submmit(new NewOrderAdd
            {
                UserId = userid,
                GoodsInfos = basket
            });

            if (result.Success)
            {
                await RedisHelper.HDelAsync(userid.ToString(), goodsId.Select(p => p.ToString()).ToArray());
            }

            //await CapBus.PublishAsync("route.basket.checkout", new CheckOut
            //{
            //    UserId = userid,
            //    Basket = basket
            //}, "CheckOutCallback");
            return (result.Success, result.Result.OrderCode, result.Error);
        }

        /// <summary>
        /// Checkout call back
        /// </summary>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        [CapSubscribe("CheckOutCallback")]
        public async Task CheckOutCallback(CheckOut checkOut)
        {
            try
            {
                if (checkOut != null)
                {
                    await RedisHelper.HDelAsync(checkOut.UserId.ToString(),
                        checkOut.Basket.Select(p => p.GoodsId.ToString()).ToArray());
                }
                else
                {
                    Logger.LogError("CheckOutCallback has error");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "CheckOutCallback");
                throw;
            }

        }
        #endregion
    }
}
