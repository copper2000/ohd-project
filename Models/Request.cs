using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Models
{
    public class Request
    {
        #nullable enable
        public int Id { get; set; }
        public int? Requestor { get; set; } = 0;
        public int? Facility { get; set; } = 0;
        public DateTime? RequestDate { get; set; } = DateTime.Now;
        public int? Assignee { get; set; } = 0;
        public int? Status { get; set; } = 0;
        public int? Severity { get; set; } = 0;
        public string? Remarks { get; set; } = "";
        public int? ClosedBy { get; set; } = 0;
        public string? Requirement { get; set; } = "";
    }
}
