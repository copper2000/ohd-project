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
    [Authorize(Roles = "3")]
    [Route("implementer")]
    public class ImplementerController : Controller
    {
        private readonly ApplicationDbContext _context;        

        public ImplementerController(ApplicationDbContext context)
        {
            _context = context;            
        }

        
        [HttpGet]
        [Route("index")]
        public ActionResult Index() // current request status
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var userId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            ViewBag.requests = from r in _context.Request
                               join f in _context.Facility
                               on r.Facility equals f.Id
                               join a in _context.Account
                               on r.Assignee equals a.Id
                               join s in _context.Status
                               on r.Status equals s.Id
                               join se in _context.Severity
                               on r.Severity equals se.Id
                               where r.Assignee == userId// request that assign to implementer
                               select new ListRequestResponse
                               {
                                   Id = r.Id,
                                   Requestor = _context.Account.FirstOrDefault(a => a.Id == r.Requestor).FullName,
                                   Facility = f.Name,
                                   RequestDate = r.RequestDate,
                                   Assignee = a.FullName,
                                   Status = s.Description,
                                   Severity = se.Description,
                                   Remarks = r.Remarks
                               };
            return View("Index");
        }
        
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            //var facility = _context.Facility.ToList();
            //var assignee = _context.Account.Where(a => a.RoleId != 1 && a.RoleId != 2).ToList();
            var status = _context.Status.Where(s => s.Id != 1 && s.Id != 2).ToList();
            //var severity = _context.Severity.ToList();
            var requestViewModel = new RequestViewModel
            {
                Request = _context.Request.Find(id),
                //Accounts = new SelectList(assignee, "Id", "Username"),
                //Facilities = new SelectList(facility, "Id", "Name"),
                Statuses = new SelectList(status, "Id", "Description"),
                //Severities = new SelectList(severity, "Id", "Description")
            };
            return View("Edit", requestViewModel);
        }
        
        [Route("edit/{id}")]
        public IActionResult Edit(int id, RequestViewModel requestViewModel)
        {
            try
            {
                var request = _context.Request.Find(id);                
                request.Status = requestViewModel.Request.Status;                

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
