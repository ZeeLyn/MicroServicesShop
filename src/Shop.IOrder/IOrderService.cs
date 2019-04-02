using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Common;
using Shop.Common.Order;
using Uragano.Abstractions;

namespace Shop.IOrder
{
    [ServiceDiscoveryName("Shop.Order")]
    [ServiceRoute("order")]
    public interface IOrderService : IService
    {
        /// <summary>
        /// submmit order
        /// </summary>
        /// <param name="order">order info</param>
        /// <returns></returns>
        [ServiceRoute("submmit")]
        Task<ResponseResult<NewOrderResult>> Submmit(NewOrderAdd order);

        /// <summary>
        /// update order status
        /// </summary>
        /// <param name="orderCode">order uid</param>
        /// <param name="status">order status</param>
        /// <returns></returns>
        [ServiceRoute("updateStatus")]
        Task<bool> UpdateOrderStatus(string orderCode, OrderStatus status, string result = "");

        /// <summary>
        /// get all order
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [ServiceRoute("list")]
        Task<List<OrderItemResult>> GetAllOrder(int userId);

        /// <summary>
        /// get specified order by order code.
        /// </summary>
        /// <param name="orderCode">order id</param>
        /// <returns></returns>
        [ServiceRoute("detail")]
        Task<(bool Succeed, OrderItemResult Order, string ErrorMessage)> GetOrder(int userId, string orderCode);
    }
}
