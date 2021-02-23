using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CAGISWebsite.Models;

namespace CAGISWebsite.Controllers
{
    public class ContestsController : Controller
    {
        private readonly CAGISKidsContext _context;

        public ContestsController(CAGISKidsContext context)
        {
            _context = context;
        }

        // GET: Contests
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contests.ToListAsync());
        }

        // GET: Contests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contests = await _context.Contests
                .FirstOrDefaultAsync(m => m.ContestId == id);
            if (contests == null)
            {
                return NotFound();
            }

            return View(contests);
        }

    }
}
