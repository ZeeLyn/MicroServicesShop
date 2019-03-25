using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Basket
{
    public class Basket : BasketBase
    {
        public string Title { get; set; }



        public string Pic { get; set; }
    }

    public class BasketBase
    {
        public decimal Price { get; set; }
        public int GoodsId { get; set; }

        public int Count { get; set; }
    }
}
