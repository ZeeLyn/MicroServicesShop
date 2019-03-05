using System;

namespace Shop.Common
{
    public class GoodsList
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Pic { get; set; }

        public decimal Price { get; set; }

    }

    public class Goods : GoodsList
    {
        public int Category { get; set; }

        public int Stock { get; set; }

        public string Description { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}
