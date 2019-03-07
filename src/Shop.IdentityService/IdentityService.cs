using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extensions;
using Shop.Common.Identity;
using Shop.IIdentity;
using Utils.Encrypt;

namespace Shop.Identity
{
    public class IdentityService : IIdentityService
    {
        private IDapper Dapper { get; }

        public IdentityService(IDapper dapper)
        {
            Dapper = dapper;
        }

        public async Task<(bool Succeed, string ErrorMessage)> Register(RegisterView userInfo)
        {
            if (await Dapper.ExecuteScalarAsync<int>("select count(1) from Users where email=@Email",
                    new { userInfo.Email }) > 0)
                return (false, "Email address already exists");
            var ok = await Dapper.ExecuteAsync("insert into Users(email,password,nickname) values(@Email,@Password,@NickName);", new
            {
                userInfo.Email,
                Password = BCryptor.Encrypt(userInfo.Password),
                userInfo.NickName
            }) > 0;
            return ok ? (true, "") : (false, "egistration failed");
        }

        public Task<AuthResult> Login(LoginView login)
        {
            throw new NotImplementedException();
        }
    }
}
