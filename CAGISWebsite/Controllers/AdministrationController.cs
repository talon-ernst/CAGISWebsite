﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAGISWebsite.Data;
using CAGISWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CAGISWebsite.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly CAGISKidsContext _context;

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                UserManager<IdentityUser> userManager, CAGISKidsContext context)
        {
            _context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
     

        public IActionResult Index()
        {
            return View();
        }

        //Deactivate existing or add new employee accounts
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ListEmployees()
        {

            var role = await roleManager.FindByNameAsync("Employee");

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Employees could not be found";
                return NotFound();
            }

            var model = new List<UserRole>();
            foreach (var user in userManager.Users)
            {
                //if (await userManager.IsInRoleAsync(user, "Employee") && !User.IsInRole("Admin"))
                //{
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };

                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRole.IsSelected = true;
                    }
                    else
                    {
                        userRole.IsSelected = false;
                    }

                    model.Add(userRole);
                //}
            }
            return View(model);
        }

        //post changes to active accounts
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ListEmployees(List<UserRole> model)
        {
            var role = await roleManager.FindByNameAsync("Employee");


            if (role == null)
            {
                ViewBag.ErrorMessage = $"Employees could not be found";
                return NotFound();
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                    continue;

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("ListEmployees");

                }
            }

            return RedirectToAction("ListEmployees");
        }

        //change password of employee account
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User could not be found";
                return NotFound();
            }

            //check if a password has been set
            var hasPassword = await userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("/Account/Manage/SetPassword");
            }

            ChangePassword changePassword = new ChangePassword
            {
                UserId = id
            };

            return View(changePassword);
        }

        //post changes to user roles
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([Bind("UserId, NewPassword, ConfirmPassword")] ChangePassword changePassword)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(changePassword.UserId);

                if (user == null)
                {
                    return NotFound($"Unable to load user'.");
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var changePasswordResult = await userManager.ResetPasswordAsync(user, token, changePassword.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    return RedirectToAction("ListEmployees");
                }
            }
            return View(changePassword);
        }

        // List all blogs
        public async Task<IActionResult> AllBlogs()
        {
            return View(await _context.Blogs.ToListAsync());
        }

        // GET: Create new blog
        public IActionResult CreateBlog()
        {
            return View();
        }

        // POST: Create new blog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog([Bind("BlogTitle,BlogText")] Blogs blogs)
        {

            if (ModelState.IsValid)
            {
                blogs.BlogId = Guid.NewGuid();
                _context.Add(blogs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllBlogs));
            }
            return View(blogs);
        }

        // GET: Edit existing blog
        public async Task<IActionResult> EditBlog(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs == null)
            {
                return NotFound();
            }
            return View(blogs);
        }

        // POST: Edit existing blog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(Guid id, [Bind("BlogId,BlogTitle,BlogText")] Blogs blogs)
        {
            if (id != blogs.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogsExists(blogs.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllBlogs));
            }
            return View(blogs);
        }

        // POST: Delete/Archive Blog
        [HttpPost, ActionName("AllBlogs")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            var blogs = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blogs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllBlogs));
        }

        //check if a blog with given id exists
        private bool BlogsExists(Guid id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }

        // List all facts
        public async Task<IActionResult> AllFacts()
        {
            return View(await _context.Facts.ToListAsync());
        }

        // GET: Create new fact
        public IActionResult CreateFact()
        {
            return View();
        }

        // POST: Create new fact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFact([Bind("Dyktitle,Dyktext")] Facts fact)
        {

            if (ModelState.IsValid)
            {
                fact.Dykid = Guid.NewGuid();
                _context.Add(fact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllFacts));
            }
            return View(fact);
        }

        // GET: Edit existing fact
        public async Task<IActionResult> EditFact(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fact = await _context.Facts.FindAsync(id);
            if (fact == null)
            {
                return NotFound();
            }
            return View(fact);
        }

        // POST: Edit existing fact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFact(Guid id, [Bind("Dykid,Dyktitle,Dyktext")] Facts fact)
        {
            if (id != fact.Dykid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FactExists(fact.Dykid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllFacts));
            }
            return View(fact);
        }

        // POST: Delete/Archive Fact
        [HttpPost, ActionName("AllFacts")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFact(Guid id)
        {
            var fact = await _context.Facts.FindAsync(id);
            _context.Facts.Remove(fact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllFacts));
        }

        //check if a fact with given id exists
        private bool FactExists(Guid id)
        {
            return _context.Facts.Any(e => e.Dykid == id);
        }

        // List all activites
        public async Task<IActionResult> AllActvities()
        {
            return View(await _context.Activities.ToListAsync());
        }

        // GET: Create new activity
        public IActionResult CreateActivity()
        {
            return View();
        }

        // POST: Create new activity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateActivity([Bind("ActivityTitle,ActivityText")] Activities activity)
        {

            if (ModelState.IsValid)
            {
                activity.ActivityId = Guid.NewGuid();
                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllActvities));
            }
            return View(activity);
        }

        // GET: Edit existing activity
        public async Task<IActionResult> EditActivity(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            return View(activity);
        }

        // POST: Edit existing activity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActivity(Guid id, [Bind("ActivityId,ActivityTitle,ActivityText")] Activities activity)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.ActivityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllActvities));
            }
            return View(activity);
        }

        // POST: Delete/Archive Activity
        [HttpPost, ActionName("AllActivities")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            var activity = await _context.Activities.FindAsync(id);
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllActvities));
        }

        //check if an activity with given id exists
        private bool ActivityExists(Guid id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }

        // List all contests
        public async Task<IActionResult> AllContests()
        {
            return View(await _context.Contests.ToListAsync());
        }

        // GET: Create new contest
        public IActionResult CreateContest()
        {
            return View();
        }

        // POST: Create new contest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContest([Bind("ContestTitle,ContestText,ContestStartDate,ContestEndDate,Email")] Contests contest)
        {

            if (ModelState.IsValid)
            {
                contest.ContestId = Guid.NewGuid();
                _context.Add(contest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllContests));
            }
            return View(contest);
        }

        // GET: Edit existing contest
        public async Task<IActionResult> EditContest(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests.FindAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            return View(contest);
        }

        // POST: Edit existing contest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContest(Guid id, [Bind("ContestTitle,ContestText,ContestStartDate,ContestEndDate,Email")] Contests contest)
        {
            if (id != contest.ContestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestExists(contest.ContestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllContests));
            }
            return View(contest);
        }

        // POST: Delete/Archive Fact
        [HttpPost, ActionName("AllContests")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContest(Guid id)
        {
            var contest = await _context.Contests.FindAsync(id);
            _context.Contests.Remove(contest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllContests));
        }

        //check if a contest with given id exists
        private bool ContestExists(Guid id)
        {
            return _context.Contests.Any(e => e.ContestId == id);
        }
    }
}