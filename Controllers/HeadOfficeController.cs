using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OHD.Data;
using OHD.Data.Constant;
using OHD.Interface;
using OHD.Models.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace OHD.Controllers
{
    public class HeadOfficeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHeadOffice _headOfficeService;

        public HeadOfficeController(ApplicationDbContext context, IHeadOffice headOffice)
        {
            _context = context;
            _headOfficeService = headOffice;
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("index")]
        public ActionResult Index() // current request status
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var requestorId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            ViewBag.requests = _headOfficeService.GetListIncomingRequest(requestorId);

            return View("Index");
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var facility = _context.Facility.ToList();
            var assignee = _context.Account.Where(a => a.RoleId != RoleConstants.ROLE_HEAD_OFFICE
                                                    && a.RoleId != RoleConstants.ROLE_STUDENT).ToList();
            var severity = _context.Severity.ToList();
            var requestViewModel = new RequestViewModel
            {
                Request = _context.Request.Find(id),
                Accounts = new SelectList(assignee, "Id", "FullName"),
                Facilities = new SelectList(facility, "Id", "Name"),
                Severities = new SelectList(severity, "Id", "Description")
            };

            return View("Edit", requestViewModel);
        }

        [Authorize(Roles = "1")]
        [Route("edit/{id}")]
        public IActionResult Edit(int id, RequestViewModel requestViewModel)
        {
            try
            {

                var result = _headOfficeService.AssignRequest(id, requestViewModel);
                if (result != null)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.msg = "Failed";
                    return View("Edit", requestViewModel);
                }
            }
            catch
            {
                ViewBag.msg = "Failed";
                return View("Edit", requestViewModel);
            }
        }
    }
}
