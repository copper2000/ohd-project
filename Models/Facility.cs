using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Models
{
    public class Facility
    {
        #nullable enable
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? SortOrder { get; set; }
    }
}
