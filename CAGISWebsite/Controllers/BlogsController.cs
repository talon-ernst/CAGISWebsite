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
    public class BlogsController : Controller
    {
        private readonly CAGISKidsContext _context;

        public BlogsController(CAGISKidsContext context)
        {
            _context = context;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            ViewData["Categories"] = TTLCategoryList(Guid.Empty);
            return View(await _context.Blogs.Include(b => b.BlogImage).OrderByDescending(b => b.BlogUploadDate).ThenBy(b => b.BlogTitle).ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs
                .Include(b => b.BlogImage).Include(b => b.BlogCategoryNavigation).FirstOrDefaultAsync(m => m.BlogId == id);
            if (blogs == null)
            {
                Archives archive = await _context.Archives.FindAsync(id);
                if (archive != null)
                {
                    return RedirectToAction("Details", "Archives", new { id });
                }
                return NotFound();
            }

            return View(blogs);
        }

        /// <summary>
        /// Function takes what user has inputted in the search box or the category dropsown
        /// and returns relevant blogs
        /// </summary>
        /// <param name="SearchPhrase"></param>
        /// <returns></returns>
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            string selectedValue = Request.Form["categoryDropdown"];
            Categories categories = new Categories();

            if (!String.IsNullOrEmpty(selectedValue))
            {
                ViewData["Categories"] = TTLCategoryList(Guid.Parse(selectedValue));
                if (selectedValue == Guid.Empty.ToString() && String.IsNullOrEmpty(SearchPhrase))
                {

                    TempData["message"] = $"Please enter something to search for, cannot be empty.";
                    return RedirectToAction("Index", "Blogs");
                }
                else
                {
                    categories = await _context.Categories
                        .FirstOrDefaultAsync(m => m.CategoryId == Guid.Parse(selectedValue));
                }               
            }

            if (String.IsNullOrEmpty(SearchPhrase) && String.IsNullOrEmpty(selectedValue))
            {
                TempData["message"] = $"Please enter something to search for, cannot be empty.";
                return RedirectToAction("Index", "Blogs");
            }
            else
            {
                if (_context.Blogs.Where(j => j.BlogTitle.Contains(SearchPhrase)).Any())
                {
                    return View("Index", await _context.Blogs.Where(b => b.BlogTitle.Contains(SearchPhrase)).Include(b => b.BlogImage).OrderByDescending(b => b.BlogUploadDate).ThenBy(b => b.BlogTitle).ToListAsync());
                }
                else if(_context.Blogs.Where(j => j.BlogCategoryNavigation.CategoryName.Contains(categories.CategoryName)).Any())
                {
                    return View("Index", await _context.Blogs.Where(j => j.BlogCategoryNavigation.CategoryName.Contains(categories.CategoryName)).Include(b => b.BlogImage).OrderByDescending(b => b.BlogUploadDate).ThenBy(b => b.BlogTitle).ToListAsync());
                }
                else
                {
                    TempData["message"] = $"There were either no search results for \"{SearchPhrase}\" or there were no results with a category of {categories.CategoryName}. Please try again!";
                    return RedirectToAction("Index", "Blogs");
                }
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
