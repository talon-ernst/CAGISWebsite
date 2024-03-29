﻿using System;
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
            //get current date and time of user
            DateTime currentDateTime = DateTime.Now;

            List<Contests> contests = await _context.Contests.Include(c => c.ContestImage)
                                        .Where(c => c.ContestStartDate <= currentDateTime)
                                        .Where(c => c.ContestEndDate > currentDateTime)
                                        .OrderByDescending(c => c.ContestStartDate)
                                        .ThenBy(c => c.ContestTitle)
                                        .ToListAsync();


            return View(contests);
        }

        // GET: Contests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contests = await _context.Contests
                .Include(c => c.ContestImage).FirstOrDefaultAsync(m => m.ContestId == id);
            if (contests == null)
            {
                Archives archive = await _context.Archives.FindAsync(id);
                if (archive != null)
                {
                    return RedirectToAction("Details", "Archives", new { id });
                }
                return NotFound();
            }
            ViewData["DateNow"] = DateTime.Now;
            return View(contests);
        }

    }
}
