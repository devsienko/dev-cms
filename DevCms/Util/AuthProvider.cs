using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace DevCms.Util
{
    public class AuthProvider : IAuthProvider
    {
        public void SignIn(HttpContext context, string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType);
            var principal = new ClaimsPrincipal(id);
            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();
        }

        public void SignOut(HttpContext context)
        {
            context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public interface IAuthProvider
    {
        void SignIn(HttpContext context, string userName);
        void SignOut(HttpContext context);
    }
}
