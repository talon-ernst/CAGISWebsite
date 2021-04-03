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
            ViewData["Categories"] = TTLCategoryList(Guid.Empty);
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
            string selectedValue = Request.Form["categoryDropdown"];
            Categories categories = new Categories();

            if (!String.IsNullOrEmpty(selectedValue))
            {
                ViewData["Categories"] = TTLCategoryList(Guid.Parse(selectedValue));
                if (selectedValue == Guid.Empty.ToString() && String.IsNullOrEmpty(SearchPhrase))
                {
                    TempData["factErrorMessage"] = $"Please enter something to search for, cannot be empty.";
                    return RedirectToAction("Index", "Facts");
                }
                else if (selectedValue == Guid.Empty.ToString() && !String.IsNullOrEmpty(SearchPhrase))
                {
                    if (_context.Facts.Where(j => j.Dyktitle.Contains(SearchPhrase)).Any())
                    {
                        return View("Index", await _context.Facts.Where(b => b.Dyktitle.Contains(SearchPhrase)).Include(b => b.Dykimage).OrderByDescending(b => b.DykuploadDate).ThenBy(b => b.Dyktitle).ToListAsync());
                    }
                    else
                    {
                        TempData["factErrorMessage"] = $"There were no search results for what you searched for. Please try again";
                        return RedirectToAction("Index", "Facts");
                    }
                }
                else
                {
                    categories = await _context.Categories
                        .FirstOrDefaultAsync(m => m.CategoryId == Guid.Parse(selectedValue));
                }
            }

            //The If returns ONLY IF both the search box and dropdown are not null
            if (_context.Facts.Where(b => b.Dyktitle.Contains(SearchPhrase)).Any() && _context.Facts.Where(b => b.DykcategoryNavigation.CategoryName.Contains(categories.CategoryName)).Any())
            {
                return View("Index", await _context.Facts.Where(b => b.Dyktitle.Contains(SearchPhrase)).Where(b => b.DykcategoryNavigation.CategoryName.Contains(categories.CategoryName)).Include(b => b.Dykimage).OrderByDescending(b => b.DykuploadDate).ThenBy(b => b.Dyktitle).ToListAsync());
            }
            //This else If returns ONLY IF the category isnt null but the search box is
            else if (_context.Facts.Where(b => b.DykcategoryNavigation.CategoryName.Contains(categories.CategoryName)).Any())
            {
                return View("Index", await _context.Facts.Where(b => b.DykcategoryNavigation.CategoryName.Contains(categories.CategoryName)).Include(b => b.Dykimage).OrderByDescending(b => b.DykuploadDate).ThenBy(b => b.Dyktitle).ToListAsync());
            }
            //Returns if both are null
            else
            {
                TempData["factErrorMessage"] = $"There were no search results for what you searched for. Please try again";
                return RedirectToAction("Index", "Facts");
            }
        }

        private SelectList TTLCategoryList(Guid selectedValue)
        {
            Categories blank = new Categories()
            {
                CategoryId = Guid.Empty,
                CategoryName = "Select A Category:"
            };
            List<Categories> categories = new List<Categories>(_context.Categories.OrderBy(c => c.CategoryName));
            categories.Add(blank);
            SelectList categorySelectList = new SelectList(categories, "CategoryId", "CategoryName", selectedValue);
            return categorySelectList;
        }

    }
}
