using OHD.Data;
using OHD.Dto;
using OHD.Interface;
using OHD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Service
{
    public class RequestService : IRequest
    {
        public ApplicationDbContext _context { get; }
        public RequestService(ApplicationDbContext context)
        {
            _context = context;
        }        

        public Request AddOrEditRequest(UpsertRequest req)
        {            
            if (req.Id == null) // create
            {
                var request = new Request
                {
                    Requestor = req.Requestor,                    
                    Facility = req.Facility,
                    RequestDate = DateTime.Now,
                    Assignee = req.Assignee,
                    Status = req.Status,
                    Remarks = req.Remarks
                };

                _context.Add(request);
                _context.SaveChanges();

                return request;
            }
            else // update
            {
                var request = _context.Request.FirstOrDefault(c => c.Id == req.Id);

                if (request == null) return null;

                req.Requestor = req.Requestor;
                req.Facility = req.Facility;                
                req.Assignee = req.Assignee;
                req.Status = req.Status;
                req.Remarks = req.Remarks;

                _context.Update(request);
                _context.SaveChanges();

                return request;
            }            
        }
    }
}
