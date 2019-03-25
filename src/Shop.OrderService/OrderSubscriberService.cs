using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extensions;
using DotNetCore.CAP;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shop.Common.Basket;
using Shop.Common.Order;

namespace Shop.OrderService
{
    public interface ISubscriberService
    {
        Task<CheckOut> CheckReceivedMessage(CheckOut model);
    }

    public class OrderSubscriberService : ISubscriberService, ICapSubscribe
    {
        private IDapper Dapper { get; }

        private ILogger Logger { get; }

        public OrderSubscriberService(IDapper dapper, ILogger<OrderSubscriberService> logger)
        {
            Dapper = dapper;
            Logger = logger;
        }

        /// <summary>
        /// submmit order
        /// </summary>
        /// <param name="order">order info</param>
        /// <returns></returns>
        public async Task<NewOrderResult> Submmit(NewOrderAdd order)
        {
            var orderCode = Guid.NewGuid().ToString("N");
            var dateNow = DateTime.Now;
            var strDateNow = dateNow.ToString("yyyy-MM-dd HH:mm:ss");
            var lstGoodsDetail = order.GoodsInfos.Select(i =>
                $"insert into `OrderDetail` (OrderCode,GoodsId,Count,Price,Amount,CreatedOn) values('{orderCode}',{i.GoodsId},{i.Count},{i.Price},{i.Count * i.Price},'{strDateNow}')");

            Dapper.BeginTransaction();
            try
            {
                await Dapper.ExecuteAsync(
                    "insert into `Order` (OrderCode,UserId,PayCode,Amount,PayStatus,OrderStatus,CreatedOn,CompletedTime) values(@OrderCode,@UserId,@PayCode,@Amount,@PayStatus,@OrderStatus,@CreatedOn,@CompletedTime);" +
                    string.Join(";", lstGoodsDetail),
                    new
                    {
                        OrderCode = orderCode,
                        order.UserId,
                        PayCode = string.Empty,
                        Amount = 0,
                        PayStatus = (int)PayStatus.UnComplete,
                        OrderStatus = (int)OrderStatus.Submmit,
                        CreatedOn = strDateNow,
                        CompletedTime = new DateTime(1999, 1, 1, 0, 0, 0)
                    });

                Dapper.CommitTransaction();
                return new NewOrderResult {CreatedOn = dateNow, OrderCode = orderCode};
            }
            catch (Exception e)
            {
                //log e.message
                Logger.LogError(e, "submmit order has error");
                Dapper.RollbackTransaction();
                return null;
            }
        }

        /// <summary>
        /// update order status
        /// </summary>
        /// <param name="orderCode">order uid</param>
        /// <param name="status">order status</param>
        /// <returns></returns>
        public async Task<bool> UpdateOrderStatus(string orderCode, OrderStatus status)
        {
            try
            {
                var order = await Dapper.QueryFirstOrDefaultAsync<NewOrderBase>(
                    "select OrderCode,OrderStatus,PayStatus from Order where OrderCode=@orderCode", new {orderCode});
                if (order.OrderStatus == status)
                {
                    //log
                    throw new Exception($"order code is ：{orderCode},updated status is the same to the old status.");
                }

                if (order.OrderStatus == OrderStatus.Delete) //deleted order cann't be handle
                {
                    //log
                    throw new Exception($"order code is ：{orderCode},deleted order cann't be handle.");
                }

                if (order.OrderStatus == OrderStatus.Cancel) //cancelled order can only be delete
                {
                    if (status != OrderStatus.Delete)
                    {
                        //log
                        throw new Exception($"order code is ：{orderCode},cancelled order can only be delete.");
                    }
                }

                if (order.OrderStatus == OrderStatus.Submmit) //submmitted order can only be cancelled
                {
                    if (status != OrderStatus.Cancel)
                    {
                        //log
                        throw new Exception($"order code is ：{orderCode},submmitted order can only be cancelled.");
                    }
                }

                if (order.OrderStatus == OrderStatus.Complete) //completed order can only be deleted
                {
                    if (status != OrderStatus.Delete)
                    {
                        //log
                        throw new Exception($"order code is ：{orderCode},completed order can only be deleted.");
                    }
                }

                Dapper.BeginTransaction();
                var result = await Dapper.ExecuteAsync(
                    "update Order set OrderStatus=@status where OrderCode=@orderCode",
                    new {orderCode});
                if (result == 1)
                {
                    Dapper.CommitTransaction();
                    return true;
                }
                else
                {
                    //log
                    Dapper.RollbackTransaction();
                    throw new Exception($"order code is ：{orderCode},order status was not changed.");
                }
            }
            catch (Exception e)
            {
                e.ToExceptionless().Submit();
                return false;
            }
        }

        [CapSubscribe("route.basket.checkout")]
        public async Task<CheckOut> CheckReceivedMessage(CheckOut model)
        {
           var result= await Submmit(new NewOrderAdd
            {
                UserId = model.UserId,
                GoodsInfos = model.Basket.Select(i => new GoodsInfo
                {
                    Count = i.Count,
                    GoodsId = i.GoodsId
                }).ToList()
            });
            return result == null ? null : model;
        }
    }
}
