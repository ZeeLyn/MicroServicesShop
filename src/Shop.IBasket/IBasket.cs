using System.Threading.Tasks;
using Uragano.Abstractions;

namespace Shop.IBasket
{
    [ServiceDiscoveryName("Shop.Basket")]
    [ServiceRoute("basket")]
    public interface IBasket
    {
        [ServiceRoute("add")]
        Task Add(int userid, int goodsId);

        [ServiceRoute("remove")]
        Task<bool> Remove(int userid, int goodsId);
    }
}
