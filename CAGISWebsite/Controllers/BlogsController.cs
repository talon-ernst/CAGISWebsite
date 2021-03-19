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
            return View(await _context.Blogs.OrderBy(j => j.BlogTitle).ThenBy(j => j.BlogUploadDate).ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blogs == null)
            {
                return NotFound();
            }

            return View(blogs);
        }

        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            if(_context.Blogs.Where(j => j.BlogTitle.Contains(SearchPhrase)).Any())
            {
                return View("Index", await _context.Blogs.Where(j => j.BlogTitle.Contains(SearchPhrase)).OrderBy(j => j.BlogTitle).ThenBy(j => j.BlogUploadDate).ToListAsync());
            }
            else
            {
                TempData["message"] = $"No search results appeared for {SearchPhrase}. Please try again!";
                return RedirectToAction("Index", "Blogs");
            }                                  
          
        }


    }
}
