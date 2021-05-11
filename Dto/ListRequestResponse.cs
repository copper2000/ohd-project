using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Dto
{
    public class ListRequestResponse
    {
        public int Id { get; set; }
        public string Requestor { get; set; }
        public string Facility { get; set; }
        public DateTime? RequestDate
        { get; set; }
        public string Assignee { get; set; }
        public string Status { get; set; }
        public string Severity { get; set; }
        public string Requirement { get; set; }
        public string Remarks { get; set; }

    }
}
