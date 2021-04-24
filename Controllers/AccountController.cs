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
using OHD.Dto;
using OHD.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OHD.Controllers
{
    [Authorize(Roles = "1, 2, 3")]
    [Route("account")]
    public class AccountController : Controller
    {
        public ApplicationDbContext _context { get; }
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("index")]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.accounts = from a in _context.Account
                               join r in _context.Role
                               on a.RoleId equals r.Id
                               where a.RoleId != 1
                               select new ListAccountResponse
                               {
                                   Id = a.Id,
                                   Email = a.Email,
                                   FullName = a.FullName,
                                   Password = a.Password,
                                   RoleId = a.RoleId,
                                   RoleName = r.Name,
                                   Status = a.Status,
                                   Username = a.Username
                               };

            return View("Index");
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("add")]
        [Route("")]
        public IActionResult Add()
        {
            var roles = _context.Role.Where(r => r.Id != 1).ToList();
            var accountViewModel = new AccountViewModel
            {
                Account = new Account(),
                Roles = new SelectList(roles, "Id", "Name")
            };

            return View("Add", accountViewModel);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        [Route("add")]
        [Route("")]
        public IActionResult Add(AccountViewModel accountViewModel)
        {
            try
            {
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(accountViewModel.Account.Password, BCrypt.Net.BCrypt.GenerateSalt());
                accountViewModel.Account.Password = hashPassword;
                _context.Account.Add(accountViewModel.Account);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Add", accountViewModel);
            }

        }

        [Authorize(Roles = "1, 2, 3")]
        [HttpGet]
        [Route("profile")]
        public IActionResult Profile()
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var account = _context.Account.SingleOrDefault(c => c.Username.Equals(username));
            return View("Profile", account);
        }

        [Authorize(Roles = "1, 2, 3")]
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
