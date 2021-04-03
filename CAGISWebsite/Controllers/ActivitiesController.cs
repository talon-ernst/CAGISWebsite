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
            ViewData["Categories"] = TTLCategoryList(Guid.Empty);
            return View(await _context.Activities.Include(a => a.ActivityImage).OrderByDescending(a => a.ActivityUploadDate).ThenBy(a => a.ActivityTitle).ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activities = await _context.Activities
                .Include(a => a.ActivityImage).Include(a => a.ActivityCategoryNavigation).FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activities == null)
            {
                Archives archive = await _context.Archives.FindAsync(id);
                if (archive != null)
                {
                    return RedirectToAction("Details", "Archives", new { id });
                }
                return NotFound();
            }

            return View(activities);
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
                    TempData["activityErrorMessage"] = $"Please enter something to search for, cannot be empty.";
                    return RedirectToAction("Index", "Activities");
                }
                else if (selectedValue == Guid.Empty.ToString() && !String.IsNullOrEmpty(SearchPhrase))
                {
                    if (_context.Activities.Where(b => b.ActivityTitle.Contains(SearchPhrase)).Any())
                    {
                        return View("Index", await _context.Activities.Where(b => b.ActivityTitle.Contains(SearchPhrase)).Include(b => b.ActivityImage).OrderByDescending(b => b.ActivityUploadDate).ThenBy(b => b.ActivityTitle).ToListAsync());
                    }
                    else
                    {
                        TempData["activityErrorMessage"] = $"There were no search results for what you searched for. Please try again";
                        return RedirectToAction("Index", "Activities");
                    }
                }
                else
                {
                    categories = await _context.Categories
                        .FirstOrDefaultAsync(m => m.CategoryId == Guid.Parse(selectedValue));
                }
            }

            //The If returns ONLY IF both the search box and dropdown are not null
            if (_context.Activities.Where(b => b.ActivityTitle.Contains(SearchPhrase)).Any() && _context.Activities.Where(b => b.ActivityCategoryNavigation.CategoryName.Contains(categories.CategoryName)).Any())
            {
                return View("Index", await _context.Activities.Where(b => b.ActivityTitle.Contains(SearchPhrase)).Where(b => b.ActivityCategoryNavigation.CategoryName.Contains(categories.CategoryName)).Include(b => b.ActivityImage).OrderByDescending(b => b.ActivityUploadDate).ThenBy(b => b.ActivityTitle).ToListAsync());
            }
            //This else If returns ONLY IF the category isnt null but the search box is
            else if (_context.Activities.Where(b => b.ActivityCategoryNavigation.CategoryName.Contains(categories.CategoryName)).Any())
            {
                return View("Index", await _context.Activities.Where(b => b.ActivityCategoryNavigation.CategoryName.Contains(categories.CategoryName)).Include(b => b.ActivityImage).OrderByDescending(b => b.ActivityUploadDate).ThenBy(b => b.ActivityTitle).ToListAsync());
            }
            //Returns if both are null
            else
            {
                TempData["activityErrorMessage"] = $"There were no search results for what you searched for. Please try again";
                return RedirectToAction("Index", "Activities");
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
