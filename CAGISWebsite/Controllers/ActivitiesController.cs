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
    public class ActivitiesController : Controller
    {
        private readonly CAGISKidsContext _context;

        public ActivitiesController(CAGISKidsContext context)
        {
            _context = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Activities.OrderBy(j => j.ActivityTitle).ThenBy(j => j.ActivityUploadDate).ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activities = await _context.Activities
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activities == null)
            {
                return NotFound();
            }

            return View(activities);
        }

        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            if (_context.Activities.Where(j => j.ActivityTitle.Contains(SearchPhrase)).Any())
            {
                return View("Index", await _context.Activities.Where(j => j.ActivityTitle.Contains(SearchPhrase)).OrderBy(j => j.ActivityTitle).ThenBy(j => j.ActivityUploadDate).ToListAsync());
            }
            else
            {
                TempData["message"] = $"No search results appeared for \"{SearchPhrase}\". Please try again!";
                return RedirectToAction("Index", "Activities");
            }
        }
    }
}
