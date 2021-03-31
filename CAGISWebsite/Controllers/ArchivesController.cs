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
    public class ArchivesController : Controller
    {
        private readonly CAGISKidsContext _context;

        public ArchivesController(CAGISKidsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //redirect user to home page if attempting to get to archive page home
            return RedirectToAction("Index", "Home");
        }

        // GET: Archives/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archives = await _context.Archives
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (archives == null)
            {
                return NotFound();
            }

            return View(archives);
        }
    }
}
