using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OHD.Data;
using OHD.Models;
using OHD.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace OHD.Controllers
{
    [Authorize(Roles = "HeadOffice")]
    [Route("account")]
    public class AccountController : Controller
    {
        public ApplicationDbContext _context { get; }
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "HeadOffice")]
        [HttpGet]
        [Route("index")]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.accounts = _context.Account.ToList();
            return View("Index");
        }

        [Authorize(Roles = "Student, Implementor, HeadOffice")]
        [HttpGet]
        [Route("profile")]
        public IActionResult Profile()
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var account = _context.Account.SingleOrDefault(c => c.Username.Equals(username));
            return View("Profile", account);
        }

        [Authorize(Roles = "Student, Implementor, HeadOffice")]
        [HttpPost]
        [Route("profile")]
        public IActionResult Profile(Account account)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name);
                var currentAccount = _context.Account.SingleOrDefault(c => c.Username.Equals(account.Username));
                currentAccount.Username = account.Username;
                currentAccount.Password = account.Password;
                if (!string.IsNullOrEmpty(account.Password))
                {
                    currentAccount.Password = BCrypt.Net.BCrypt
                        .HashPassword(account.Password, BCrypt.Net.BCrypt.GenerateSalt());
                }
                currentAccount.FullName = account.FullName;
                currentAccount.Email = account.Email;
                _context.SaveChanges();

                return View("Profile", currentAccount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
