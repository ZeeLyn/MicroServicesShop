using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shop.Common;
using Uragano.Abstractions;

namespace Shop.IGoods
{
    [ServiceDiscoveryName("Shop.Goods")]
    [ServiceRoute("goodsCategory")]
    public interface IGoodsCategoryService : IService
    {
        [ServiceRoute("CategoryList")]
        Task<List<GoodsCategoryBase>> List();
    }
}
