using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OHD.Data;
using OHD.Dto;
using OHD.Interface;
using OHD.Models;
using OHD.Models.ViewModels;
using System.Collections.Generic;
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
                               where r.Requestor == requestorId && s.Id != 4
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

        [Authorize(Roles = "2")]
        [HttpGet]
        [Route("history")]
        public ActionResult HistoryRequest() // list history request, only open and closed status
        {
            var username = User.FindFirst(ClaimTypes.Name);
            var requestorId = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value)).Id;
            ViewBag.requests = from r in _context.HistoryRequest
                               join f in _context.Facility
                               on r.Facility equals f.Id
                               join a in _context.Account
                               on r.Assignee equals a.Id
                               join s in _context.Status
                               on r.Status equals s.Id
                               join se in _context.Severity
                               on r.Severity equals se.Id
                               where r.Requestor == requestorId && (s.Id == 3 || s.Id == 4)
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
            return View("HistoryRequest");
        }

        [Authorize(Roles = "2")]
        [HttpGet]
        [Route("addOrUpdate")]        
        public ActionResult Add()
        {
            //var assignee = _context.Account.Where(a => a.RoleId != 1 && a.RoleId != 2).ToList();
            //var status = _context.Status.ToList();
            //Accounts = new SelectList(assignee, "Id", "Username"),
            //Statuses = new SelectList(status, "Id", "Description"),
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
                requestViewModel.Request.Requestor = requestorId;
                requestViewModel.Request.Status = 1;
                requestViewModel.Request.Assignee = 1; // head_office_id -> add to constant later
                var createdRequest = _context.Request.Add(requestViewModel.Request);
                if(createdRequest.Entity != null)
                {
                    _context.SaveChanges();
                    var historyRequest = new HistoryRequest
                    {
                        Id = createdRequest.Entity.Id,
                        Requestor = requestorId,
                        Facility = (int)createdRequest.Entity.Facility,
                        RequestDate = (System.DateTime)createdRequest.Entity.RequestDate,
                        Assignee = (int)createdRequest.Entity.Assignee,
                        Status = (int)createdRequest.Entity.Status,
                        Severity = (int)createdRequest.Entity.Severity,
                        Remarks = createdRequest.Entity.Remarks                  
                    };
                    _context.HistoryRequest.Add(historyRequest);
                    _context.SaveChanges();
                }                    
                else
                {
                    return null;
                }
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Add", requestViewModel);
            }
        }        

        [Authorize(Roles = "1, 2, 3")]
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var facility = _context.Facility.ToList();
            //var assignee = _context.Account.Where(a => a.RoleId != 1 && a.RoleId != 2).ToList();
            //var status = _context.Status.ToList();
            var severity = _context.Severity.ToList();
            var requestViewModel = new RequestViewModel
            {
                Request = _context.Request.Find(id),
                //Accounts = new SelectList(assignee, "Id", "Username"),
                Facilities = new SelectList(facility, "Id", "Name"),
                //Statuses = new SelectList(status, "Id", "Description"),
                Severities = new SelectList(severity, "Id", "Description")
            };
            return View("Edit", requestViewModel);            
        }

        [Authorize(Roles = "2")]
        [Route("edit/{id}")]
        public IActionResult Edit(int id, RequestViewModel requestViewModel)
        {
            try
            {
                var request = _context.Request.Find(id);                
                request.Facility = requestViewModel.Request.Facility;
                request.RequestDate = requestViewModel.Request.RequestDate;
                request.Severity = requestViewModel.Request.Severity;
                request.Assignee = 1; // head_office_id -> add to constant later                
                request.Remarks = requestViewModel.Request.Remarks;
                
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
        [HttpGet]
        [Route("close-request/{id}")]
        public IActionResult CloseRequest(int id, RequestViewModel requestViewModel)
        {
            try
            {
                var request = _context.Request.Find(id);                               
                requestViewModel.Request.Status = 4; // closed request        
                _context.Request.Update(request);
                _context.SaveChanges();

                return RedirectToAction("Index");
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
                var request = _context.Request.Find(id);
                _context.Request.Remove(request);
                _context.SaveChanges();
                ViewBag.msg = "Done";
            }
            catch
            {
                ViewBag.msg = "Failed";
            }

            return RedirectToAction("Index");
        }
    }
}
