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
    [Authorize(Roles = "1, 2, 3")]
    [Route("request")]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRequest _request;

        public RequestController(ApplicationDbContext context, IRequest request)
        {
            _context = context;
            _request = request;
        }

        [Authorize(Roles = "2")]
        [HttpGet]
        [Route("index")]
        public ActionResult Index()
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var requestorId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            ViewBag.requests = _request.GetListRequest(requestorId);

            return View("Index");
        }

        [Authorize(Roles = "2")]
        [HttpGet]
        [Route("history")]
        public ActionResult HistoryRequest() // list history request, only open and closed status
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var requestorId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            ViewBag.requests = _request.GetListHistoryRequest(requestorId);

            return View("HistoryRequest");
        }

        [Authorize(Roles = "2")]
        [HttpGet]
        [Route("addOrUpdate")]
        public ActionResult Add()
        {
            var facility = _context.Facility.ToList();
            var severity = _context.Severity.ToList();
            var requestViewModel = new RequestViewModel
            {
                Facilities = new SelectList(facility, "Id", "Name"),
                Severities = new SelectList(severity, "Id", "Description")
            };
            return View("Add", requestViewModel);
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        [Route("addOrUpdate")]
        public ActionResult AddOrUpdate(RequestViewModel requestViewModel)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name);
                var requestorId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;

                var result = _request.AddRequest(requestViewModel, requestorId);
                if (result != null)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.msg = "Failed";
                    return View("Add", requestViewModel);
                }
            }
            catch
            {
                ViewBag.msg = "Failed";
                return View("Add", requestViewModel);
            }
        }

        [Authorize(Roles = "1, 2, 3")]
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var facility = _context.Facility.ToList();
            var severity = _context.Severity.ToList();
            var requestViewModel = new RequestViewModel
            {
                Request = _context.Request.Find(id),
                Facilities = new SelectList(facility, "Id", "Name"),
                Severities = new SelectList(severity, "Id", "Description")
            };
            return View("Edit", requestViewModel);
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        [Route("edit/{id}")]
        public IActionResult Edit(int id, RequestViewModel requestViewModel)
        {
            try
            {
                var result = _request.EditRequest(id, requestViewModel);
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

        [Authorize(Roles = "2")]
        [Route("close-request/{id}")]
        public IActionResult CloseRequest(int id)
        {

            var reason = _context.Reason.ToList();
            var requestViewModel = new RequestViewModel
            {
                Request = _context.Request.Find(id),
                Reasons = new SelectList(reason, "Id", "Description"),
            };

            return View("CloseRequest", requestViewModel);
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        [Route("close-request")]
        public IActionResult CloseRequest(int id, RequestViewModel requestViewModel)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name);
                var requestorId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;

                var result = _request.CloseRequest(id, requestorId, requestViewModel);
                if (result)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.msg = "Failed";
                    return View("CloseRequest", requestViewModel);
                }
            }
            catch
            {
                ViewBag.msg = "Failed";
                return View("CloseRequest", requestViewModel);
            }
        }

        // GET: HomeController1/Delete/5
        [Authorize(Roles = "1, 2, 3")]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _request.DeleteRequest(id);
                if (result)
                {
                    ViewBag.msg = "Done";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.msg = "Failed";
            }

            return RedirectToAction("Index");
        }
    }
}
