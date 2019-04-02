using System.Threading.Tasks;
using Shop.Common;
using Shop.Common.Identity;
using Uragano.Abstractions;

namespace Shop.IIdentity
{
    [ServiceDiscoveryName("Shop.Identity")]
    [ServiceRoute("identity")]
    public interface IIdentityService : IService
    {
        [ServiceRoute("register")]
        Task<(bool Succeed, string ErrorMessage)> Register(RegisterView userInfo);

        [ServiceRoute("login")]
        Task<(bool Succeed, string ErrorMessage, AuthResult Result)> Login(LoginView login);

        [ServiceRoute("info")]
        Task<ResponseResult<UserBase>> UserInfo(int userId);
    }
}
