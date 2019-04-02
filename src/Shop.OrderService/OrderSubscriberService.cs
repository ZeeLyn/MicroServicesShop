using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Extensions;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Shop.Common.Basket;
using Shop.Common.Order;
using Shop.IIdentity;
using FluentEmail.Core;
using Newtonsoft.Json;
using Shop.Common.Identity;

namespace Shop.OrderService
{
    public interface ISubscriberService
    {
        Task<bool> SendEmailMessage(OrderUser userOrder);
    }

    public class OrderSubscriberService : ISubscriberService, ICapSubscribe
    {
        private ILogger Logger { get; }
        private IIdentityService IdentityService { get; }
        private IFluentEmail Email { get; }

        public OrderSubscriberService(IIdentityService identityService, ILogger<OrderSubscriberService> logger,
            IFluentEmail email)
        {
            IdentityService = identityService;
            Logger = logger;
            Email = email;
        }

        [CapSubscribe("route.order.email")]
        public async Task<bool> SendEmailMessage(OrderUser userOrder)
        {
            try
            {
                //get user info
                var userInfo = await IdentityService.UserInfo(userOrder.UserId);
                if (userInfo.Success)
                {
                    var nickName = userInfo.Result.NickName;
                    var email = userInfo.Result.Email;
                    string subject = string.Empty;
                    string content = string.Empty;
                    switch (userOrder.OrderStatus)
                    {
                        case OrderStatus.Failed:
                            subject = "order failed.";
                            content = "has been failed.";
                            break;
                        case OrderStatus.Cancel:
                            subject = "order cancelled.";
                            content = "has been cancelled.";
                            break;
                        case OrderStatus.Submmit:
                            subject = "order submmited.";
                            content = "has been submmited.";
                            break;
                    }

                    //send email
                    await Email.To(email).Subject(subject)
                        .Body($"Dear {nickName},order {userOrder.OrderCode} {content}").SendAsync();
                    return true;
                }

                Logger.LogWarning(
                    $"get user failed when sending email.order info is:{JsonConvert.SerializeObject(userOrder)}");
                return false;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "send email failed.");
                return false;
            }
        }
    }
}
