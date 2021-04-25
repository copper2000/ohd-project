using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Models.ViewModels
{
    public class RequestViewModel
   {
        public Account Account { get; set; }
        public Request Request { get; set; }
        public SelectList Facilities { get; set; }
        //public SelectList Statuses { get; set; }
        public SelectList Severities { get; set; }
        public SelectList Accounts { get; set; }
        
    }
}
