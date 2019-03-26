using System;
using System.Threading.Tasks;
using Shop.Common.Order;
using Uragano.Abstractions;

namespace Shop.IOrder
{
    [ServiceDiscoveryName("Shop.Order")]
    [ServiceRoute("order")]
    public interface IOrderService:IService
    {
        /// <summary>
        /// submmit order
        /// </summary>
        /// <param name="order">order info</param>
        /// <returns></returns>
        [ServiceRoute("submmit")]
        Task<NewOrderResult> Submmit(NewOrderAdd order);

        /// <summary>
        /// update order status
        /// </summary>
        /// <param name="orderCode">order uid</param>
        /// <param name="status">order status</param>
        /// <returns></returns>
        [ServiceRoute("updateStatus")]
        Task<bool> UpdateOrderStatus(string orderCode, OrderStatus status);
    }
}
