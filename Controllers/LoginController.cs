using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OHD.Data;
using OHD.Models;
using OHD.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Controllers
{
    //[Authorize(Roles = "Student")]
    [Route("login")]
    public class LoginController : Controller
    {
        public ApplicationDbContext _context { get; }
        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("index")]
        [Route("")]
        [Route("~/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("process")]
        public IActionResult Process(string username, string password)
        {
            var account = Check(username, password);
            if (account != null)
            {
                var securityManager = new SecurityManager();
                securityManager.SignIn(HttpContext, account);
                return RedirectToAction("Index", "Dashboard");
            } else
            {
                ViewBag.error = "Invalid";
                return View("Index");
            }            
        }

        [Route("SignOut")]
        public IActionResult SingOut()
        {
            var securityManager = new SecurityManager();
            securityManager.SignOut(HttpContext);
            return RedirectToAction("Index");
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("AccessDenied");
        }

        private Account Check(string username, string password)
        {
            var account = _context.Account.SingleOrDefault(account => account.Username.Equals(username));

            if (account != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, account.Password))
                {
                    return account;
                }
            }

            return null;
        }
    }
}
