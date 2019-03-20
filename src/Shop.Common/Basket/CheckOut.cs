using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Basket
{
    public class CheckOut
    {
        public int UserId { get; set; }

        public List<Basket> Basket { get; set; }
    }
}
