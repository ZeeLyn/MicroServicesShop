using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Order
{
    public class NewOrder
    {
        /// <summary>
        /// order uid
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// user id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// pay uid
        /// </summary>
        public string PayCode { get; set; }

        /// <summary>
        /// order lump-sum price
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// pay status
        /// </summary>
        public int PayStatus { get; set; }

        /// <summary>
        /// order status
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// order submitted time
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// order completed time
        /// </summary>
        public DateTime CompletedTime { get; set; }
    }

    public class NewOrderDetail
    {
        /// <summary>
        /// order uid
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// goods uid
        /// </summary>
        public int GoodsId { get; set; }

        /// <summary>
        /// goods count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// goods unit-price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// goods lump-sum price
        /// </summary>
        public decimal Amount { get; set; }
    }
}
