using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OHD.Data;
using OHD.Models;
using System;
using System.Linq;
using System.Security.Claims;
using OHD.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using OHD.Interface;
using OHD.Dto;

namespace OHD.Controllers
{
    [Authorize(Roles = "1, 2, 3")]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccount _accountService;
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context, IAccount account)
        {
            _context = context;
            _accountService = account;
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("index")]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.accounts = _accountService.GetListAccount();

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
        public IActionResult Add(AccountViewModel accountViewModel)
        {
            try
            {
                var result = _accountService.AddAccount(accountViewModel);
                if (result != null)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.msg = "Failed";
                    return View("Add", accountViewModel);
                }
            }
            catch
            {
                ViewBag.msg = "Failed";
                return View("Add", accountViewModel);
            }
        }

        [Authorize(Roles = "1")]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _accountService.DeleteAccount(id);
                if (result)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.msg = "Failed";
                }
            }
            catch
            {
                ViewBag.msg = "Failed";
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var roles = _context.Role.Where(r => r.Id != 1).ToList();
            var accountViewModel = new AccountViewModel
            {
                Account = _context.Account.Find(id),
                Roles = new SelectList(roles, "Id", "Name")
            };

            return View("Edit", accountViewModel);
        }

        [Authorize(Roles = "1")]
        [Route("edit/{id}")]
        public IActionResult Edit(int id, AccountViewModel accountViewModel)
        {
            try
            {
                var result = _accountService.EditAccount(id, accountViewModel);
                if (result != null)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.msg = "Failed";
                    return View("Edit", accountViewModel);
                }
            }
            catch
            {
                ViewBag.msg = "Failed";
                return View("Edit", accountViewModel);
            }
        }

        [Authorize(Roles = "1, 2, 3")]
        [HttpGet]
        [Route("profile")]
        public IActionResult Profile()
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var userId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            var account = _context.Account.SingleOrDefault(c => c.Id == userId);
            var roleName = _context.Role.FirstOrDefault(x => x.Id == account.RoleId)?.Name;

            ListAccountResponse response = new ListAccountResponse
            {
               Username = account.Username,
               Password = account.Password,
               FullName = account.FullName,
               Status = account.Status,
               Email = account.Email,                
               RoleName = roleName
            };

            return View("Profile", response);
        }

        [Authorize(Roles = "1, 2, 3")]
        [HttpPost]
        [Route("profile")]
        public IActionResult Profile(Account account)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name);

                var result = _accountService.EditProfile(username, account);
                if (result != null)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.msg = "Failed";
                    return View("Profile", result);
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "Failed";
                throw ex;
            }
        }
    }
}
