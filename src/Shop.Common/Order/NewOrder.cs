using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Order
{
    public class NewOrderAdd
    {
        /// <summary>
        /// user id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// goods info list
        /// </summary>
        public List<GoodsInfo> GoodsInfos { get; set; }
    }
    /// <summary>
    /// new order
    /// </summary>
    public class NewOrder: NewOrderAdd
    {
        ///// <summary>
        ///// user id
        ///// </summary>
        //public int UserId { get; set; }

        ///// <summary>
        ///// pay uid
        ///// </summary>
        //public string PayCode { get; set; }

        ///// <summary>
        ///// order lump-sum price
        ///// </summary>
        //public decimal Amount { get; set; }

        ///// <summary>
        ///// pay status
        ///// </summary>
        //public PayStatus PayStatus { get; set; }

        /// <summary>
        /// order status
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// order submitted time
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// order completed time
        /// </summary>
        public DateTime CompletedTime { get; set; }
        ///// <summary>
        ///// goods info list
        ///// </summary>
        //public List<GoodsInfo> GoodsInfos { get; set; }
    }

    public class OrderLstResult
    {
        /// <summary>
        /// order uid
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// order status
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        
        /// <summary>
        /// order submitted time
        /// </summary>
        public DateTime CreatedOn { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// goods list
        /// </summary>
        public List<GoodsInfoObj> GoodsInfos { get; set; }
        public string Result { get; set; }
    }

    public class OrderItemResult: OrderLstResult
    {
        /// <summary>
        /// pay status
        /// </summary>
        public PayStatus PayStatus { get; set; }
    }
    
    public class NewOrderResult
    {
        /// <summary>
        /// order uid
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// order submitted time
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }

    public class NewOrderBase
    {
        /// <summary>
        /// order uid
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// order status
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// pay status
        /// </summary>
        public PayStatus PayStatus { get; set; }
    }

    /// <summary>
    /// new order detail
    /// </summary>
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
    }

    public class GoodsInfo: GoodsInfoBase
    {

        /// <summary>
        /// goods unit-price
        /// </summary>
        public decimal Price { get; set; }
    }

    public class GoodsInfoBase
    {
        /// <summary>
        /// goods uid
        /// </summary>
        public int GoodsId { get; set; }
        /// <summary>
        /// goods count
        /// </summary>
        public int Count { get; set; }
    }

    public class GoodsInfoObj: GoodsInfo
    {
        public string Pic { get; set; }
        public string Title { get; set; }
    }

    public class OrderPublish
    {
        public int UserId { get; set; }
        /// <summary>
        /// order uid
        /// </summary>
        public string OrderCode { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        /// <summary>
        /// goods info list
        /// </summary>
        public List<GoodsInfoBase> GoodsInfos { get; set; }
    }

    public class OrderUser
    {
        /// <summary>
        /// order uid
        /// </summary>
        public string OrderCode { get; set; }
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    /// <summary>
    /// order status enum
    /// </summary>
    public enum OrderStatus
    {
        Delete = -2,
        Failed = -1,
        Cancel = 0,
        Submmit = 1,
        Complete = 2
    }

    /// <summary>
    /// pay status enum
    /// </summary>
    public enum PayStatus
    {
        Refund = -1,
        UnComplete,
        Complete
    }
}
