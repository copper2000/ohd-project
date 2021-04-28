using Microsoft.AspNetCore.Mvc.Rendering;

namespace OHD.Models.ViewModels
{
    public class RequestViewModel
   {
        public Account Account { get; set; }
        public Request Request { get; set; }
        public SelectList Facilities { get; set; }
        public SelectList Statuses { get; set; }
        public SelectList Severities { get; set; }
        public SelectList Accounts { get; set; }
        public SelectList Reasons { get; set; }
        
    }
}
