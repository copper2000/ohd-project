using OHD.Data;
using OHD.Data.Constant;
using OHD.Dto;
using OHD.Interface;
using OHD.Models;
using OHD.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OHD.Service
{
    public class RequestService : IRequest
    {
        public ApplicationDbContext _context { get; }
        public RequestService(ApplicationDbContext context)
        {
            _context = context;
        }      

        public IQueryable<ListRequestResponse> GetListRequest(int requestorId)
        {
            var lstRequest = from r in _context.Request
                             join f in _context.Facility
                             on r.Facility equals f.Id
                             join a in _context.Account
                             on r.Assignee equals a.Id
                             join s in _context.Status
                             on r.Status equals s.Id
                             join se in _context.Severity
                             on r.Severity equals se.Id
                             where r.Requestor == requestorId
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

            return lstRequest;
        }

        public IQueryable<ListRequestResponse> GetListHistoryRequest(int requestorId)
        {
            var lstHistoryRequest = from r in _context.HistoryRequest
                                    join f in _context.Facility
                                    on r.Facility equals f.Id
                                    join a in _context.Account
                                    on r.Assignee equals a.Id
                                    join s in _context.Status
                                    on r.Status equals s.Id
                                    join se in _context.Severity
                                    on r.Severity equals se.Id
                                    where r.Requestor == requestorId && (r.Status == StatusConstants.STATUS_WORK_IN_PROGRESS || r.Status == StatusConstants.STATUS_CLOSED)
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

            return lstHistoryRequest;
        }

        public Request AddRequest(RequestViewModel requestViewModel, int requestorId)
        {
            requestViewModel.Request.Requestor = requestorId;
            requestViewModel.Request.Status = StatusConstants.STATUS_UNASSIGNED;
            requestViewModel.Request.RequestDate = DateTime.Now;
            requestViewModel.Request.Assignee = RoleConstants.ROLE_HEAD_OFFICE;

            var createdRequest = _context.Request.Add(requestViewModel.Request);
            if (createdRequest.Entity != null)
            {
                _context.SaveChanges();
                var historyRequest = new HistoryRequest
                {
                    Id = createdRequest.Entity.Id,
                    Requestor = requestorId,
                    Facility = (int)createdRequest.Entity.Facility,
                    RequestDate = DateTime.Now,
                    Assignee = (int)createdRequest.Entity.Assignee,
                    Status = (int)createdRequest.Entity.Status,
                    Severity = (int)createdRequest.Entity.Severity,
                    Remarks = createdRequest.Entity.Remarks,
                    Requirement = createdRequest.Entity.Requirement
                };
                _context.HistoryRequest.Add(historyRequest);
                _context.SaveChanges();

                return requestViewModel.Request;
            }
            else
            {
                return null;
            }
        }

        public Request EditRequest(int requestId, RequestViewModel requestViewModel)
        {
            var request = _context.Request.Find(requestId);
            request.Facility = requestViewModel.Request.Facility;
            request.RequestDate = DateTime.Now;
            request.Severity = requestViewModel.Request.Severity;
            request.Assignee = RoleConstants.ROLE_HEAD_OFFICE;
            request.Remarks = requestViewModel.Request.Remarks;
            request.Requirement = requestViewModel.Request.Requirement;

            _context.Request.Update(request);
            _context.SaveChanges();

            return request;
        }        

        public bool CloseRequest(int requestId, int requestorId, RequestViewModel requestViewModel)
        {
            var request = _context.Request.Find(requestId);
            request.Status = StatusConstants.STATUS_CLOSED;
            request.Assignee = null;
            request.ClosedBy = requestorId;

            _context.Request.Update(request);
            _context.SaveChanges();

            var historyRequest = _context.HistoryRequest.Find(request.Id);
            historyRequest.Status = StatusConstants.STATUS_CLOSED;

            _context.HistoryRequest.Update(historyRequest);
            _context.SaveChanges();

            return true;
        }

        public bool DeleteRequest(int requestId)
        {
            var request = _context.Request.Find(requestId);
            _context.Request.Remove(request);
            _context.SaveChanges();

            return true;
        }
    }
}
