using System;
using System.Threading.Tasks;
using Uragano.Abstractions;

namespace Shop.IOrder
{
    [ServiceDiscoveryName("Shop.Order")]
    [ServiceRoute("order")]
    public interface IOrder
    {
        //Task<>
    }
}
