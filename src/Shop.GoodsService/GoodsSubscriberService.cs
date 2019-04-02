using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extensions;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Shop.Common;
using Shop.Common.Order;

namespace Shop.GoodsService
{
    public interface IGoodsSubscribeService
    {
        Task<ResponseResult<OrderPublish>> UpdateStock(OrderPublish order);
    }

    public class GoodsSubscriberService : IGoodsSubscribeService, ICapSubscribe
    {
        private IDapper Dapper { get; }

        private ILogger Logger { get; }

        public GoodsSubscriberService(IDapper dapper, ILogger<GoodsSubscriberService> logger)
        {
            Dapper = dapper;
            Logger = logger;
        }

        [CapSubscribe("route.order.submmit")]
        public async Task<ResponseResult<OrderPublish>> UpdateStock(OrderPublish order)
        {
            Dapper.BeginTransaction();
            try
            {
                foreach (var goodsInfo in order.GoodsInfos)
                {
                    var tempGoods = goodsInfo;
                    var result = await Dapper.ExecuteAsync(
                        $"update `Goods` set Stock=Stock-{tempGoods.Count} where Id={tempGoods.GoodsId} and Stock>={tempGoods.Count};");
                    if (result != 1)
                    {
                        Dapper.RollbackTransaction();
                        return new ResponseResult<OrderPublish> { Error = "stock is low", Result = order, Success = false };
                    }
                }
                Dapper.CommitTransaction();
                return new ResponseResult<OrderPublish> { Error = "", Result = order, Success = true };
            }
            catch (Exception e)
            {
                Logger.LogError(e, "update stock has error");
                Dapper.RollbackTransaction();
                return new ResponseResult<OrderPublish> { Error = "update stock has error", Result = order, Success = false };
            }
        }
    }
}
