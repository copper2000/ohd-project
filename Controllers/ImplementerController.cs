using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OHD.Data;
using OHD.Interface;
using OHD.Models.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace OHD.Controllers
{
    [Authorize(Roles = "3")]
    [Route("implementer")]
    public class ImplementerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImplementer _implementerService;

        public ImplementerController(ApplicationDbContext context, IImplementer implementer)
        {
            _context = context;
            _implementerService = implementer;
        }


        [HttpGet]
        [Route("index")]
        public ActionResult Index() // current request status
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var userId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            ViewBag.requests = _implementerService.GetListAssignedRequest(userId);

            return View("Index");
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var status = _context.Status.Where(s => s.Id != StatusConstants.STATUS_UNASSIGNED
                                                 && s.Id != StatusConstants.STATUS_ASSIGNED && s.Id != StatusConstants.STATUS_CLOSED)
                                        .ToList();

            var requestViewModel = new RequestViewModel
            {
                Request = _context.Request.Find(id),
                Statuses = new SelectList(status, "Id", "Description"),
            };

            return View("Edit", requestViewModel);
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id, RequestViewModel requestViewModel)
        {
            try
            {
                var result = _implementerService.UpdateRequestStatus(id, requestViewModel);
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
