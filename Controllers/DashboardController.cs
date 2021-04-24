using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OHD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHD.Controllers
{
    [Authorize(Roles = "1, 2, 3")]
    [Route("Dashboard")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Index")]
        [Route("")]        
        public IActionResult Index()
        {
            var lstRole = _context.Role.ToList();
            return View();
        }
    }
}
