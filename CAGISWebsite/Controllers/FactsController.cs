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
    public class FactsController : Controller
    {
        private readonly CAGISKidsContext _context;

        public FactsController(CAGISKidsContext context)
        {
            _context = context;
        }

        // GET: Facts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Facts.Include(f => f.Dykimage).OrderByDescending(f => f.DykuploadDate).ThenBy(f => f.Dyktitle).ToListAsync());
        }

        // GET: Facts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facts = await _context.Facts
                .Include(f => f.Dykimage).Include(f => f.DykcategoryNavigation).FirstOrDefaultAsync(m => m.Dykid == id);
            if (facts == null)
            {
                Archives archive = await _context.Archives.FindAsync(id);
                if (archive != null)
                {
                    return RedirectToAction("Details", "Archives", new { id });
                }
                return NotFound();
            }

            return View(facts);
        }

        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            if (_context.Facts.Where(j => j.Dyktitle.Contains(SearchPhrase)).Any())
            {
                return View("Index", await _context.Facts.Where(j => j.Dyktitle.Contains(SearchPhrase)).Include(f => f.Dykimage).OrderByDescending(f => f.DykuploadDate).ThenBy(f => f.Dyktitle).ToListAsync());
            }
            else
            {
                TempData["message"] = $"No search results appeared for {SearchPhrase}. Please try again!";
                return RedirectToAction("Index", "Facts");
            }
        }

    }
}
