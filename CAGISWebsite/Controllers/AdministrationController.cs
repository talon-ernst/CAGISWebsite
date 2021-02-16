using System;
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
    //[Authorize]
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


    }
}
