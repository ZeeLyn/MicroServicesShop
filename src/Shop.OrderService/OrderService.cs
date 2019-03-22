using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Extensions;
using DotNetCore.CAP;
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

        public OrderService(IDapper dapper)
        {
            Dapper = dapper;
        }

        /// <summary>
        /// submmit order
        /// </summary>
        /// <param name="order">order info</param>
        /// <returns></returns>
        public async Task<NewOrderResult> Submmit(NewOrder order)
        {
            var orderCode = Guid.NewGuid().ToString("N");
            var dateNow = DateTime.Now;
            var lstGoodsDetail = order.GoodsInfos.Select(i =>
                $"insert into OrderDetail(OrderCode,GoodsId,Count,Price,Amount,CreatedOn) values('{orderCode}',{i.GoodsId},{i.Count},{i.Price},{i.Count * i.Price},'{dateNow}')");

            Dapper.BeginTransaction();
            try
            {
                await Dapper.ExecuteAsync(
                    "insert into Order(OrderCode,UserId,PayCode,Amount,PayStatus,OrderStatus,CreatedOn,CompletedTime) values(@OrderCode,@UserId,@PayCode,@Amount,@PayStatus,@OrderStatus,@CreatedOn,@CompletedTime);" +
                    string.Join(";", lstGoodsDetail),
                    new
                    {
                        orderCode,
                        order.UserId,
                        PayCode = string.Empty,
                        Amount = order.GoodsInfos.Sum(i => i.Price * i.Count),
                        PayStatus = PayStatus.UnComplete,
                        CreatedOn = dateNow,
                        CompletedTime = new DateTime(1999, 1, 1, 0, 0, 0)
                    });

                Dapper.CommitTransaction();
                return new NewOrderResult {CreatedOn = dateNow, OrderCode = orderCode};
            }
            catch (Exception e)
            {
                //log e.message
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
            var order = await Dapper.QueryFirstOrDefaultAsync<NewOrderBase>(
                "select OrderCode,OrderStatus,PayStatus from Order where OrderCode=@orderCode", new {orderCode});
            if (order.OrderStatus == status)
            {
                //log
                return false;
            }

            if (order.OrderStatus == OrderStatus.Delete) //deleted order cann't be handle
            {
                //log
                return false;
            }

            if (order.OrderStatus == OrderStatus.Cancel) //cancelled order can only be delete
            {
                if (status != OrderStatus.Delete)
                {
                    //log
                    return false;
                }
            }

            if (order.OrderStatus == OrderStatus.Submmit) //submmitted order can only be cancelled
            {
                if (status != OrderStatus.Cancel)
                {
                    //log
                    return false;
                }
            }

            if (order.OrderStatus == OrderStatus.Complete) //completed order can only be delete
            {
                if (status != OrderStatus.Delete)
                {
                    //log
                    return false;
                }
            }

            Dapper.BeginTransaction();
            var result = await Dapper.ExecuteAsync("update Order set OrderStatus=@status where OrderCode=@orderCode",
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
                return false;
            }

        }

    }
}
