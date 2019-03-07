using System.Threading.Tasks;
using Shop.Common.Identity;
using Uragano.Abstractions;

namespace Shop.IIdentity
{
    [ServiceDiscoveryName("Shop.Identity")]
    [ServiceRoute("identity")]
    public interface IIdentityService
    {
        Task<(bool Succeed, string ErrorMessage)> Register(RegisterView userInfo);

        Task<AuthResult> Login(LoginView login);
    }
}
