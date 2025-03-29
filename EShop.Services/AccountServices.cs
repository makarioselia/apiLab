﻿using EF_Core.Models;
using EShop.Manegers;
using EShop.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;


namespace EShop.Services
{
    public class AccountServices 
    {
        AccountManager accountManager;
        VendorManager vendorManager;
        ClientManager clientManager;
        IConfiguration appSettingConfiguration;

        public AccountServices( AccountManager _accountManager,VendorManager _vendorManager,ClientManager _clientManager,IConfiguration configuration)
        {
            accountManager = _accountManager;
            vendorManager = _vendorManager;
            clientManager = _clientManager;
            appSettingConfiguration = configuration;
        }

        public  async Task<IdentityResult> CreateAccount(UserRegisterVM user )
        {
            var userRes =  await accountManager.Register(user);
            
            if (userRes.Succeeded)
            {
                var currentUser = await accountManager.FindByUserName(user.UserName);
                if (user.Role == "vendor")
                {
                    vendorManager.Add(new Vendor() { UserId = currentUser.Id });
                    return IdentityResult.Success;
                }
                else if (user.Role == "client")
                {
                    clientManager.Add(new Client { UserId = currentUser.Id });
                    return IdentityResult.Success;
                }
            }
            return IdentityResult.Failed();
        }

        public async Task<SignInResult> Login(UserLoginVM user)
        {
            return await accountManager.Login(user);
        }

        public async Task<string> LoginWithToken(UserLoginVM User)
        {
            var res = await accountManager.Login(User);
            if (res.Succeeded)
            {
                List<Claim> UserData = new List<Claim>();
                var currentUser = await accountManager.FindByUserName(User.UserTryLoginName);
                if (currentUser == null)
                {
                    currentUser = await accountManager.FindByEmail(User.UserTryLoginName);
                }

                var roles = await accountManager.GetUserRoles(currentUser);

                UserData.Add(new Claim(ClaimTypes.Name, currentUser.UserName));
                UserData.Add(new Claim(ClaimTypes.NameIdentifier, currentUser.Id));

                UserData.Add(new Claim(ClaimTypes.Email, currentUser.Email));
                roles.ForEach(role => UserData.Add(new Claim(ClaimTypes.Role, role)));

                JwtSecurityToken securityToken = new JwtSecurityToken(
                 claims: UserData,
                 expires: DateTime.Now.AddMonths(1),
                  signingCredentials: new SigningCredentials(
                            algorithm: SecurityAlgorithms.HmacSha256,
                            key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettingConfiguration["JWT:PrivateKey"]))));

                return new JwtSecurityTokenHandler().WriteToken(securityToken);
            }

            else if (res.IsLockedOut || res.IsNotAllowed)
            {
                return string.Empty;
            }
            return null;
        }

        public async Task Signout()
        {
            await accountManager.Signout();
        }
    }
}
