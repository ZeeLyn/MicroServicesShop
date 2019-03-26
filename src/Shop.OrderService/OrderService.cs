using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Extensions;
using DotNetCore.CAP;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Shop.Common;
using Shop.Common.Basket;
using Shop.Common.Order;
using Shop.IOrder;

namespace Shop.OrderService
{
    /// <summary>
    /// order service implementation
    /// </summary>
    public class OrderService : IOrderService
    {
        private IDapper Dapper { get; }
        private ILogger Logger { get; }
        public OrderService(IDapper dapper, ILogger<OrderSubscriberService> logger)
        {
            Dapper = dapper;
            Logger = logger;
        }

        /// <summary>
        /// submmit order
        /// </summary>
        /// <param name="order">order info</param>
        /// <returns></returns>
        public async Task<ResponseResult<NewOrderResult>> Submmit(NewOrderAdd order)
        {
            var orderCode = Guid.NewGuid().ToString("N");
            var dateNow = DateTime.Now;
            var strDateNow = dateNow.ToString("yyyy-MM-dd HH:mm:ss");
            var lstGoodsDetail = order.GoodsInfos.Select(i =>
                $"insert into `OrderDetail` (OrderCode,GoodsId,Count,Price,CreatedOn) values('{orderCode}',{i.GoodsId},{i.Count},{i.Price},'{strDateNow}')");

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
                        Amount = order.GoodsInfos.Sum(i => i.Price * i.Count),
                        PayStatus = (int) PayStatus.UnComplete,
                        OrderStatus = (int) OrderStatus.Submmit,
                        CreatedOn = strDateNow,
                        CompletedTime = new DateTime(1999, 1, 1, 0, 0, 0)
                    });

                Dapper.CommitTransaction();
                return new ResponseResult<NewOrderResult>
                {
                    Success = true,
                    Result = new NewOrderResult {CreatedOn = dateNow, OrderCode = orderCode},
                    Error = ""
                };
            }
            catch (Exception e)
            {
                //log e.message
                Logger.LogError(e, "submmit order has error");
                Dapper.RollbackTransaction();
                return new ResponseResult<NewOrderResult>
                {
                    Success = false,
                    Result = null,
                    Error = "submmit order has error"
                };
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
                    "select OrderCode,OrderStatus,PayStatus from Order where OrderCode=@orderCode", new { orderCode });
                if (order.OrderStatus == status)
                {
                    //log
                    Logger.LogError($"order code is ：{orderCode},updated status is the same to the old status.");
                    return false;
                }

                if (order.OrderStatus == OrderStatus.Delete) //deleted order cann't be handle
                {
                    //log
                    Logger.LogError($"order code is ：{orderCode},deleted order cann't be handle.");
                    return false;
                }

                if (order.OrderStatus == OrderStatus.Cancel) //cancelled order can only be delete
                {
                    if (status != OrderStatus.Delete)
                    {
                        //log
                        Logger.LogError($"order code is ：{orderCode},cancelled order can only be delete.");
                        return false;
                    }
                }

                if (order.OrderStatus == OrderStatus.Submmit) //submmitted order can only be cancelled
                {
                    if (status != OrderStatus.Cancel)
                    {
                        //log
                        Logger.LogError($"order code is ：{orderCode},submmitted order can only be cancelled.");
                        return false;
                    }
                }

                if (order.OrderStatus == OrderStatus.Complete) //completed order can only be deleted
                {
                    if (status != OrderStatus.Delete)
                    {
                        //log
                        Logger.LogError($"order code is ：{orderCode},completed order can only be deleted.");
                        return false;
                    }
                }

                Dapper.BeginTransaction();
                var result = await Dapper.ExecuteAsync(
                    "update Order set OrderStatus=@status where OrderCode=@orderCode",
                    new { orderCode });
                if (result == 1)
                {
                    Dapper.CommitTransaction();
                    return true;
                }
                else
                {
                    //log
                    Logger.LogError($"order code is ：{orderCode},order status was not changed.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"order code is ：{orderCode},order status changed has error.");
                return false;
            }
        }

    }
}
