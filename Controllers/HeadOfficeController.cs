using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OHD.Data;
using OHD.Dto;
using OHD.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OHD.Controllers
{
    public class HeadOfficeController : Controller
    {
        private readonly ApplicationDbContext _context;        

        public HeadOfficeController(ApplicationDbContext context)
        {
            _context = context;            
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("index")]
        public ActionResult Index() // current request status
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var requestorId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            ViewBag.requests = from r in _context.Request
                               join f in _context.Facility
                               on r.Facility equals f.Id
                               join a in _context.Account
                               on r.Assignee equals a.Id
                               join s in _context.Status
                               on r.Status equals s.Id
                               join se in _context.Severity
                               on r.Severity equals se.Id
                               where r.Assignee == 1 // request that assign to head office first
                               select new ListRequestResponse
                               {
                                   Id = r.Id,
                                   Requestor = _context.Account.FirstOrDefault(a => a.Id == r.Requestor).FullName,
                                   Facility = f.Name,
                                   RequestDate = r.RequestDate,
                                   Assignee = a.Username,
                                   Status = s.Description,
                                   Severity = se.Description,
                                   Remarks = r.Remarks
                               };
            return View("Index");
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var facility = _context.Facility.ToList();
            var assignee = _context.Account.Where(a => a.RoleId != 1 && a.RoleId != 2).ToList();
            //var status = _context.Status.ToList();
            var severity = _context.Severity.ToList();
            var requestViewModel = new RequestViewModel
            {
                Request = _context.Request.Find(id),
                Accounts = new SelectList(assignee, "Id", "Username"),
                Facilities = new SelectList(facility, "Id", "Name"),
                //Statuses = new SelectList(status, "Id", "Description"),
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
                var request = _context.Request.Find(id);
                //request.Requestor = requestViewModel.Request.Requestor;
                request.Facility = requestViewModel.Request.Facility;
                request.RequestDate = requestViewModel.Request.RequestDate;
                requestViewModel.Request.Assignee = requestViewModel.Request.Assignee;
                //request.Status = requestViewModel.Request.Status;
                request.Remarks = requestViewModel.Request.Remarks;
                request.Assignee = requestViewModel.Request.Assignee;

                _context.Request.Update(request);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.msg = "Failed";
                return View("Edit", requestViewModel);
            }
        }
    }
}
