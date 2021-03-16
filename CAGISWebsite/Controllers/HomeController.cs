using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CAGISWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace CAGISWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CAGISKidsContext _context;


        public HomeController(ILogger<HomeController> logger, CAGISKidsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeModel homeModel = new HomeModel();
            homeModel.Blogs = _context.Blogs.Include(b => b.BlogImage).Take(5);
            homeModel.Activities = _context.Activities.Include(b => b.ActivityImage).Take(5);
            homeModel.Facts = _context.Facts.Include(b => b.Dykimage).Take(5);
            return View(homeModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
