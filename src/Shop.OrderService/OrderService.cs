using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Extensions;
using Microsoft.Extensions.Logging;
using Shop.Common;
using Shop.Common.Order;
using Shop.IGoods;
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
        private IGoodsService GoodsService { get; }

        public OrderService(IDapper dapper, ILogger<OrderSubscriberService> logger, IGoodsService goodsService)
        {
            Dapper = dapper;
            Logger = logger;
            GoodsService = goodsService;
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
                    "select OrderCode,OrderStatus,PayStatus from Order where OrderCode=@orderCode", new {orderCode});
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
                    new {orderCode});
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

        /// <summary>
        /// get all orders from user id.
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public async Task<List<OrderItemResult>> GetAllOrder(int userId)
        {
            var lstOrder = await Dapper.QueryAsync<OrderItemResult>(
                "select OrderCode,OrderStatus,PayStatus,CreatedOn from `Order` where UserId=@userId order by CreatedOn desc",
                new {userId});
            var lstCode = lstOrder.Select(i => i.OrderCode).ToList();
            var lstOrderDetail = await Dapper.QueryAsync<NewOrderDetail>(
                "select OrderCode,GoodsId,Count,Price from OrderDetail where OrderCode in @lstCode", new {lstCode});
            var lstGoodsId = lstOrderDetail.Select(i => i.GoodsId).ToList();
            var lstGoods = await GoodsService.GoodsInfos(lstGoodsId);
            var result = new List<OrderItemResult>();
            lstOrder.ForEach(i =>
            {
                var order = new OrderItemResult
                {
                    CreatedOn = i.CreatedOn,
                    OrderCode = i.OrderCode,
                    OrderStatus = i.OrderStatus,
                    PayStatus = i.PayStatus,
                    GoodsInfos = new List<GoodsInfoObj>(),
                };
                var lstDetail = lstOrderDetail.Where(j => j.OrderCode == i.OrderCode).ToList();
                lstDetail.ForEach(j =>
                {
                    var srcGoods = lstGoods.FirstOrDefault(k => k.Id == j.GoodsId);
                    order.GoodsInfos.Add(new GoodsInfoObj
                    {
                        Count = j.Count,
                        GoodsId = j.GoodsId,
                        Price = j.Price,
                        Pic = srcGoods?.Pic,
                        Title = srcGoods?.Title
                    });
                });
                order.Amount = order.GoodsInfos.Sum(k => k.Count * k.Price);
                result.Add(order);
            });
            return result;
        }

        /// <summary>
        /// get specified order by order code.
        /// </summary>
        /// <param name="orderCode">order id</param>
        /// <returns></returns>
        public async Task<(bool Succeed, OrderItemResult Order, string ErrorMessage)> GetOrder(int userId,
            string orderCode)
        {
            var order = await Dapper.QueryFirstOrDefaultAsync<OrderItemResult>(
                "select OrderCode,OrderStatus,PayStatus,CreatedOn from `Order` where OrderCode=@orderCode and UserId=@userId",
                new {orderCode, userId});
            if (order == null) return (false, null, "order not exists");
            var lstDetail = await Dapper.QueryAsync<NewOrderDetail>(
                "select OrderCode,GoodsId,Count,Price from OrderDetail where OrderCode=@orderCode", new {orderCode});
            var lstGoodsId = lstDetail.Select(i => i.GoodsId).ToList();
            var lstGoods = await GoodsService.GoodsInfos(lstGoodsId);
            order.GoodsInfos = new List<GoodsInfoObj>();
            lstDetail.ForEach(j =>
            {
                var srcGoods = lstGoods.FirstOrDefault(k => k.Id == j.GoodsId);
                order.GoodsInfos.Add(new GoodsInfoObj
                {
                    Count = j.Count,
                    GoodsId = j.GoodsId,
                    Price = j.Price,
                    Pic = srcGoods?.Pic,
                    Title = srcGoods?.Title
                });
            });
            order.Amount = order.GoodsInfos.Sum(k => k.Count * k.Price);
            return (true, order, "");
        }
    }
}
