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
        [ServiceRoute("GoodsList")]
        Task<List<GoodsList>> GoodsList(int category);
        [ServiceRoute("info")]
        Task<GoodsList> GoodsInfo(int id);
        [ServiceRoute("baseInfos")]
        Task<List<GoodsList>> GoodsInfos(IEnumerable<int> ids);
    }
}
