using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Common.Basket;
using Uragano.Abstractions;

namespace Shop.IBasket
{
    [ServiceDiscoveryName("Shop.Basket")]
    [ServiceRoute("basket")]
    public interface IBasketService : IService
    {
        [ServiceRoute("add")]
        Task Add(int userid, int goodsId, int count);

        [ServiceRoute("remove")]
        Task Remove(int userid, int goodsId);

        Task<List<Basket>> Get(int userid);


        Task<int> Count(int userid);

        Task<bool> CheckOut(int userid, List<int> goodsId);
    }
}
