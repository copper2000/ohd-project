using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OHD.Data;
using OHD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OHD.Security
{
    public class SecurityManager : Controller
    {
        private readonly ApplicationDbContext _context;
        public SecurityManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public SecurityManager()
        {
        }

        public async void SignIn(HttpContext httpContext, Account account)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(GetUserClaims(account),
                CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }

        public async void SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
        }

        private IEnumerable<Claim> GetUserClaims(Account account)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, account.Username));            
            claims.Add(new Claim(ClaimTypes.Role, account.RoleId.ToString()));
            return claims;
        }
    }
}
