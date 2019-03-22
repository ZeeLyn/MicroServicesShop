using System;
using System.Collections.Generic;
using System.Text;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Shop.Common.Basket;

namespace Shop.OrderService
{
    public interface ISubscriberService
    {
        void CheckReceivedMessage(CheckOut model);
    }

    public class OrderSubscriberService : ISubscriberService, ICapSubscribe
    {
        [CapSubscribe("route.basket.checkout")]
        public void CheckReceivedMessage(CheckOut model)
        {
            Console.WriteLine(
                $@"{DateTime.Now}, Subscriber invoked, Sent time:{model.UserId};{
                        JsonConvert.SerializeObject(model.Basket)
                    }");
        }
    }
}
