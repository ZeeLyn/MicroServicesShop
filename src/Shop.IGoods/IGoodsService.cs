using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Common;
using Uragano.Abstractions;

namespace Shop.IGoods
{
    [ServiceDiscoveryName("Shop.Goods")]
    [ServiceRoute("goods")]
    public interface IGoodsService : IService
    {
        [ServiceRoute("list")]
        Task<List<GoodsList>> List(int category);
    }
}
