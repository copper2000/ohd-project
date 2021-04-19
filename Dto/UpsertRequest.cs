using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Dto
{
    public class UpsertRequest
    {
        public int? Id { get; set; }
        public int? Requestor { get; set; }
        public int? Facility { get; set; }        
        public int? Assignee { get; set; }
        public int? Status { get; set; }
        public string Remarks { get; set; }
    }
}
