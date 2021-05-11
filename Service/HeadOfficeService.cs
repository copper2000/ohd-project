using OHD.Data;
using OHD.Dto;
using OHD.Interface;
using OHD.Models;
using OHD.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace OHD.Service
{
    public class HeadOfficeService : IHeadOffice
    {
        private readonly ApplicationDbContext _context;

        public HeadOfficeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Request AssignRequest(int requestId, RequestViewModel requestViewModel)
        {
            var request = _context.Request.Find(requestId);
            request.Facility = requestViewModel.Request.Facility;
            request.Assignee = requestViewModel.Request.Assignee;
            request.Status = StatusConstants.STATUS_ASSIGNED;
            request.Remarks = requestViewModel.Request.Remarks;
            request.Assignee = requestViewModel.Request.Assignee;

            _context.Request.Update(request);
            _context.SaveChanges();

            return request;
        }

        public IQueryable<ListRequestResponse> GetListIncomingRequest(int requestorId)
        {
            var lstIncomingRequest = from r in _context.Request
                                     join f in _context.Facility
                                     on r.Facility equals f.Id
                                     join a in _context.Account
                                     on r.Assignee equals a.Id
                                     join s in _context.Status
                                     on r.Status equals s.Id
                                     join se in _context.Severity
                                     on r.Severity equals se.Id
                                     select new ListRequestResponse
                                     {
                                         Id = r.Id,
                                         Requestor = _context.Account.FirstOrDefault(a => a.Id == r.Requestor).FullName,
                                         Facility = f.Name,
                                         RequestDate = r.RequestDate,
                                         Assignee = a.FullName,
                                         Status = s.Description,
                                         Severity = se.Description,
                                         Remarks = r.Remarks,
                                         Requirement = r.Requirement
                                     };

            return lstIncomingRequest;
        }
    }
}
