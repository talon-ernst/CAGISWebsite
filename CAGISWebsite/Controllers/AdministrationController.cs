using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CAGISWebsite.Data;
using CAGISWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IWebHostEnvironment webHostEnvironment;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                UserManager<IdentityUser> userManager, CAGISKidsContext context,
                                IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
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
                Employees employee = _context.Employees.Where(e => e.AdminId == Guid.Parse(user.Id)).FirstOrDefault();
                //only include admin and activated employees in the list
                if ((await userManager.IsInRoleAsync(user, "Employee") && employee.IsActivated))
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };

                    //in case an account gets through somehow
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRole.IsSelected = true;
                    }
                    else
                    {
                        userRole.IsSelected = false;
                    }

                    model.Add(userRole);
                }
            }
            var sortedModel = model.OrderBy(x => x.UserName).ToList();

            return View(sortedModel);
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

                //remove designated accounts from the activated list
                if (!model[i].IsSelected)
                {
                    Employees employee = _context.Employees.Where(e => e.AdminId == Guid.Parse(user.Id)).FirstOrDefault();

                    employee.IsActivated = false;
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                else
                    continue;

                if (i < (model.Count - 1))
                    continue;
                else
                    return RedirectToAction("ListEmployees");
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
            return View(await _context.Blogs.Include(b => b.BlogImage).OrderByDescending(b => b.BlogUploadDate).ThenBy(b => b.BlogTitle).ToListAsync());
        }

        // GET: Create new blog
        public IActionResult CreateBlog()
        {
            ViewData["Categories"] = TTLCategoryList(Guid.Empty);
            return View();
        }

        // POST: Create new blog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog([Bind("BlogTitle,BlogText,BlogCategory,CategoryName")] BlogCategories blogs, IFormFile file, string blogStatus)
        {
            //check if image is valid
            ValidImageUpload(file, "Blog");

            if (blogStatus == "CreateBlog")
            {
                if (blogs.BlogCategory == Guid.Empty)
                {
                    ModelState.AddModelError("CategoryName", "Add or select a category before creating a blog");
                }

                if (ModelState.IsValid)
                {
                    //create new blog object
                    Blogs newBlog = new Blogs();
                    //give new blog a unique id
                    newBlog.BlogId = Guid.NewGuid();
                    newBlog.BlogTitle = blogs.BlogTitle;
                    newBlog.BlogText = blogs.BlogText;
                    newBlog.BlogCategory = blogs.BlogCategory;

                    //add image to image folder if employee uploaded one
                    if (file != null)
                    {
                        Images image = new Images();
                        var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                        int counter = 1;

                        //check if image share name of other images
                        while (System.IO.File.Exists("wwwroot" + filePath))
                        {
                            //add incremented value
                            string newFilePath = file.FileName.Replace(".", $"({counter}).");
                            filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                            counter++;
                        }
                        //add image to project
                        var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        //add to database
                        image.ImageId = Guid.NewGuid();
                        image.ImagePath = filePath;
                        _context.Add(image);
                        await _context.SaveChangesAsync();

                        newBlog.BlogImageId = image.ImageId;
                    }
                    //set blog upload date
                    newBlog.BlogUploadDate = DateTime.Now;
                    newBlog.BlogEditDate = DateTime.Now;
                    _context.Add(newBlog);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AllBlogs));
                }
            }

            //Add New Category
            else
            {
                if (ModelState.IsValid)
                {
                    Categories category = new Categories();
                    category.CategoryId = Guid.NewGuid();
                    category.CategoryName = blogs.CategoryName;
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    blogs.BlogCategory = category.CategoryId;
                } 
            }
            ViewData["Categories"] = TTLCategoryList(blogs.BlogCategory);
            return View();
        }

        // GET: Edit existing blog
        public async Task<IActionResult> EditBlog(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs.Include(b => b.BlogImage).Where(b => b.BlogId.Equals(id)).FirstOrDefaultAsync();
            if (blogs == null)
            {
                return NotFound();
            }
            BlogCategories blogCategory = new BlogCategories
            {
                BlogId = blogs.BlogId,
                BlogTitle = blogs.BlogTitle,
                BlogText = blogs.BlogText,
                BlogImageId = blogs.BlogImageId,
                BlogCategory = blogs.BlogCategory
            };
            
            if(blogs.BlogImageId != null)
                ViewData["ImagePath"] = blogs.BlogImage.ImagePath;
            ViewData["Categories"] = TTLCategoryList(blogCategory.BlogCategory);
            return View(blogCategory);
        }

        // POST: Edit existing blog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(Guid id, [Bind("BlogId,BlogTitle,BlogText,BlogCategory,CategoryName,BlogImageId")] BlogCategories blogs, IFormFile file, string blogStatus)
        {
            if (id != blogs.BlogId)
            {
                return NotFound();
            }
            //push changes to database
            if (blogStatus == "EditBlog")
            {
                //check if image is valid
                ValidImageUpload(file, "Blog");

                if (blogs.BlogCategory == Guid.Empty)
                {
                    ModelState.AddModelError("CategoryName", "Add or select a category before creating a blog");
                }

                if (ModelState.IsValid)
                {
                    var editBlogs = await _context.Blogs.Where(b => b.BlogId.Equals(id)).FirstOrDefaultAsync();
                    editBlogs.BlogTitle = blogs.BlogTitle;
                    editBlogs.BlogText = blogs.BlogText;
                    editBlogs.BlogCategory = blogs.BlogCategory;

                    //add image to image folder if employee uploaded one
                    if (file != null)
                    {
                        Images image = new Images();
                        var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                        int counter = 1;

                        //check if image share name of other images
                        while (System.IO.File.Exists("wwwroot" + filePath))
                        {
                            //add incremented value
                            string newFilePath = file.FileName.Replace(".", $"({counter}).");
                            filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                            counter++;
                        }
                        //add image to project
                        var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        //add to database
                        image.ImageId = Guid.NewGuid();
                        image.ImagePath = filePath;
                        _context.Add(image);
                        await _context.SaveChangesAsync();

                        editBlogs.BlogImageId = image.ImageId;
                    }
                    //set blog edit date
                    editBlogs.BlogEditDate = DateTime.Now;
                    try
                    {
                        _context.Update(editBlogs);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BlogsExists(editBlogs.BlogId))
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
            }
            //Add New Category
            else if(blogStatus == "AddCategory")
            {
                if (ModelState.IsValid)
                {
                    Categories category = new Categories();
                    category.CategoryId = Guid.NewGuid();
                    category.CategoryName = blogs.CategoryName;
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    blogs.BlogCategory = category.CategoryId;
                }
            }

            //Remove Image From Blog
            else
            {
                blogs.BlogImageId = null;
            }
            if (blogs.BlogImageId != null)
                ViewData["ImagePath"] = _context.Images.Where(i => i.ImageId.Equals(blogs.BlogImageId)).FirstOrDefault().ImagePath;
            ViewData["Categories"] = TTLCategoryList(blogs.BlogCategory);
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
            return View(await _context.Facts.Include(f => f.Dykimage).OrderByDescending(f => f.DykuploadDate).ThenBy(f => f.Dyktitle).ToListAsync());
        }

        // GET: Create new fact
        public IActionResult CreateFact()
        {
            ViewData["Categories"] = TTLCategoryList(Guid.Empty);
            return View();
        }

        // POST: Create new fact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFact([Bind("Dyktitle,Dyktext,Dykcategory,CategoryName")] FactCategories fact, IFormFile file, string factStatus)
        {
            if (factStatus == "CreateFact")
            {
                if (fact.Dykcategory == Guid.Empty)
                {
                    ModelState.AddModelError("CategoryName", "Add or select a category before creating a Did You Know");
                }
                //check if image is valid
                ValidImageUpload(file, "Dyk");
                if (ModelState.IsValid)
                {
                    //create new fact object
                    Facts newFact = new Facts();
                    //give new Did You Know? a unique id
                    newFact.Dykid = Guid.NewGuid();
                    newFact.Dyktitle = fact.Dyktitle;
                    newFact.Dyktext = fact.Dyktext;
                    newFact.Dykcategory = fact.Dykcategory;

                    //add image to image folder if employee uploaded one
                    if (file != null)
                    {
                        Images image = new Images();
                        var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                        int counter = 1;

                        //check if image share name of other images
                        while (System.IO.File.Exists("wwwroot" + filePath))
                        {
                            //add incremented value
                            string newFilePath = file.FileName.Replace(".", $"({counter}).");
                            filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                            counter++;
                        }
                        //add image to project
                        var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        //add to database
                        image.ImageId = Guid.NewGuid();
                        image.ImagePath = filePath;
                        _context.Add(image);
                        await _context.SaveChangesAsync();

                        newFact.DykimageId = image.ImageId;
                    }
                    //set DYK upload date
                    newFact.DykuploadDate = DateTime.Now;
                    newFact.DykeditDate = DateTime.Now;
                    _context.Add(newFact);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AllFacts));
                }
            }
            //Add New Category
            else
            {
                if (ModelState.IsValid)
                {
                    Categories category = new Categories();
                    category.CategoryId = Guid.NewGuid();
                    category.CategoryName = fact.CategoryName;
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    fact.Dykcategory = category.CategoryId;
                }
            }
            ViewData["Categories"] = TTLCategoryList(fact.Dykcategory);
            return View(fact);
        }

        // GET: Edit existing fact
        public async Task<IActionResult> EditFact(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fact = await _context.Facts.Include(f => f.Dykimage).Where(f => f.Dykid.Equals(id)).FirstOrDefaultAsync();
            if (fact == null)
            {
                return NotFound();
            }
            FactCategories factCategory = new FactCategories
            {
                Dykid = fact.Dykid,
                Dyktitle = fact.Dyktitle,
                Dyktext = fact.Dyktext,
                DykimageId = fact.DykimageId,
                Dykcategory = fact.Dykcategory
            };

            if (fact.DykimageId != null)
                ViewData["ImagePath"] = fact.Dykimage.ImagePath;
            ViewData["Categories"] = TTLCategoryList(factCategory.Dykcategory);
            return View(factCategory);
        }

        // POST: Edit existing fact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFact(Guid id, [Bind("Dykid,Dyktitle,Dyktext,Dykcategory,CategoryName,DykimageId")] FactCategories fact, IFormFile file, string factStatus)
        {
            if (id != fact.Dykid)
            {
                return NotFound();
            }

            if (factStatus == "EditFact")
            {
                if (fact.Dykcategory == Guid.Empty)
                {
                    ModelState.AddModelError("CategoryName", "Add or select a category before creating a Did You Know");
                }
                //check if image is valid
                ValidImageUpload(file, "Dyk");

                if (ModelState.IsValid)
                {
                    var editFact = await _context.Facts.Where(f => f.Dykid.Equals(id)).FirstOrDefaultAsync();
                    editFact.Dyktitle = fact.Dyktitle;
                    editFact.Dyktext = fact.Dyktext;
                    editFact.Dykcategory = fact.Dykcategory;
                    //add image to image folder if employee uploaded one
                    if (file != null)
                    {
                        Images image = new Images();
                        var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                        int counter = 1;

                        //check if image share name of other images
                        while (System.IO.File.Exists("wwwroot" + filePath))
                        {
                            //add incremented value
                            string newFilePath = file.FileName.Replace(".", $"({counter}).");
                            filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                            counter++;
                        }
                        //add image to project
                        var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        //add to database
                        image.ImageId = Guid.NewGuid();
                        image.ImagePath = filePath;
                        _context.Add(image);
                        await _context.SaveChangesAsync();

                        editFact.DykimageId = image.ImageId;
                    }
                    //set DYK edit date
                    editFact.DykeditDate = DateTime.Now;
                    try
                    {
                        _context.Update(editFact);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FactExists(editFact.Dykid))
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
            }
            //Add New Category
            else if (factStatus == "AddCategory")
            {
                if (ModelState.IsValid)
                {
                    Categories category = new Categories();
                    category.CategoryId = Guid.NewGuid();
                    category.CategoryName = fact.CategoryName;
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    fact.Dykcategory = category.CategoryId;
                }
            }

            //Remove Image From Fact
            else
            {
                fact.DykimageId = null;
            }

            if (fact.DykimageId != null)
                ViewData["ImagePath"] = _context.Images.Where(i => i.ImageId.Equals(fact.DykimageId)).FirstOrDefault().ImagePath;
            ViewData["Categories"] = TTLCategoryList(fact.Dykcategory);
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

        // List all activities
        public async Task<IActionResult> AllActivities()
        {
            return View(await _context.Activities.Include(a => a.ActivityImage).OrderByDescending(a => a.ActivityUploadDate).ThenBy(a => a.ActivityTitle).ToListAsync());
        }

        // GET: Create new activity
        public IActionResult CreateActivity()
        {
            ViewData["Categories"] = TTLCategoryList(Guid.Empty);
            return View();
        }

        // POST: Create new activity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateActivity([Bind("ActivityTitle,ActivityText,ActivityCategory,CategoryName")] ActivityCategories activity, IFormFile file, string activityStatus)
        {
            if (activityStatus == "CreateActivity")
            {
                if (activity.ActivityCategory == Guid.Empty)
                {
                    ModelState.AddModelError("CategoryName", "Add or select a category before creating an activity");
                }
                //check if image is valid
                ValidImageUpload(file, "Activity");

                if (ModelState.IsValid)
                {
                    //create new activity object
                    Activities newActivity = new Activities();
                    //give new activity a unique id
                    newActivity.ActivityId = Guid.NewGuid();
                    newActivity.ActivityTitle = activity.ActivityTitle;
                    newActivity.ActivityText = activity.ActivityText;
                    newActivity.ActivityCategory = activity.ActivityCategory;

                    //add image to image folder if employee uploaded one
                    if (file != null)
                    {
                        Images image = new Images();
                        var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                        int counter = 1;

                        //check if image share name of other images
                        while (System.IO.File.Exists("wwwroot" + filePath))
                        {
                            //add incremented value
                            string newFilePath = file.FileName.Replace(".", $"({counter}).");
                            filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                            counter++;
                        }
                        //add image to project
                        var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        //add to database
                        image.ImageId = Guid.NewGuid();
                        image.ImagePath = filePath;
                        _context.Add(image);
                        await _context.SaveChangesAsync();

                        newActivity.ActivityImageId = image.ImageId;
                    }
                    //set activity upload date
                    newActivity.ActivityUploadDate = DateTime.Now;
                    newActivity.ActivityEditDate = DateTime.Now;
                    _context.Add(newActivity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AllActivities));
                }
            }
            //Add New Category
            else
            {
                if (ModelState.IsValid)
                {
                    Categories category = new Categories();
                    category.CategoryId = Guid.NewGuid();
                    category.CategoryName = activity.CategoryName;
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    activity.ActivityCategory = category.CategoryId;
                }
            }
            ViewData["Categories"] = TTLCategoryList(activity.ActivityCategory);
            return View(activity);
        }

        // GET: Edit existing activity
        public async Task<IActionResult> EditActivity(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.Include(a => a.ActivityImage).Where(a => a.ActivityId.Equals(id)).FirstOrDefaultAsync();
            if (activity == null)
            {
                return NotFound();
            }
            ActivityCategories activityCategory = new ActivityCategories
            {
                ActivityId = activity.ActivityId,
                ActivityTitle = activity.ActivityTitle,
                ActivityText = activity.ActivityText,
                ActivityImageId = activity.ActivityImageId,
                ActivityCategory = activity.ActivityCategory
            };

            if (activity.ActivityImageId != null)
                ViewData["ImagePath"] = activity.ActivityImage.ImagePath;
            ViewData["Categories"] = TTLCategoryList(activityCategory.ActivityCategory);
            return View(activityCategory);
        }

        // POST: Edit existing activity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActivity(Guid id, [Bind("ActivityId,ActivityTitle,ActivityText,ActivityCategory,CategoryName,ActivityImageId")] ActivityCategories activity, IFormFile file, string activityStatus)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (activityStatus == "EditActivity")
            {
                if (activity.ActivityCategory == Guid.Empty)
                {
                    ModelState.AddModelError("CategoryName", "Add or select a category before creating an activity");
                }
                //check if image is valid
                ValidImageUpload(file, "Activity");

                if (ModelState.IsValid)
                {
                    var editActivity = await _context.Activities.Where(a => a.ActivityId.Equals(id)).FirstOrDefaultAsync();
                    editActivity.ActivityTitle = activity.ActivityTitle;
                    editActivity.ActivityText = activity.ActivityText;
                    editActivity.ActivityCategory = activity.ActivityCategory;
                    //add image to image folder if employee uploaded one
                    if (file != null)
                    {
                        Images image = new Images();
                        var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                        int counter = 1;

                        //check if image share name of other images
                        while (System.IO.File.Exists("wwwroot" + filePath))
                        {
                            //add incremented value
                            string newFilePath = file.FileName.Replace(".", $"({counter}).");
                            filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                            counter++;
                        }
                        //add image to project
                        var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        //add to database
                        image.ImageId = Guid.NewGuid();
                        image.ImagePath = filePath;
                        _context.Add(image);
                        await _context.SaveChangesAsync();

                        editActivity.ActivityImageId = image.ImageId;
                    }
                    //set activity edit date
                    editActivity.ActivityEditDate = DateTime.Now;
                    try
                    {
                        _context.Update(editActivity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ActivityExists(editActivity.ActivityId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(AllActivities));
                }
            }
            //Add New Category
            else if (activityStatus == "AddCategory")
            {
                if (ModelState.IsValid)
                {
                    Categories category = new Categories();
                    category.CategoryId = Guid.NewGuid();
                    category.CategoryName = activity.CategoryName;
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    activity.ActivityCategory = category.CategoryId;
                }
            }
            //Remove Image from Activity
            else
            {
                activity.ActivityImageId = null;
            }
            if (activity.ActivityImageId != null)
                ViewData["ImagePath"] = _context.Images.Where(i => i.ImageId.Equals(activity.ActivityImageId)).FirstOrDefault().ImagePath;
            ViewData["Categories"] = TTLCategoryList(activity.ActivityCategory);
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
            return RedirectToAction(nameof(AllActivities));
        }

        //check if an activity with given id exists
        private bool ActivityExists(Guid id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }

        // List all contests
        public async Task<IActionResult> AllContests()
        {
            return View(await _context.Contests.Include(c => c.ContestImage).OrderByDescending(c => c.ContestUploadDate).ThenBy(c => c.ContestTitle).ToListAsync());
        }

        // GET: Create new contest
        public IActionResult CreateContest()
        {
            return View();
        }

        // POST: Create new contest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContest([Bind("ContestTitle,ContestText,ContestStartDate,ContestEndDate,Email")] Contests contest, IFormFile file)
        {
            //check if image is valid
            ValidImageUpload(file, "Contest");

            if (ModelState.IsValid)
            {
                //give new contest a unique id
                contest.ContestId = Guid.NewGuid();

                //add image to image folder if employee uploaded one
                if (file != null)
                {
                    Images image = new Images();
                    var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                    int counter = 1;

                    //check if image share name of other images
                    while (System.IO.File.Exists("wwwroot" + filePath))
                    {
                        //add incremented value
                        string newFilePath = file.FileName.Replace(".", $"({counter}).");
                        filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                        counter++;
                    }
                    //add image to project
                    var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    //add to database
                    image.ImageId = Guid.NewGuid();
                    image.ImagePath = filePath;
                    _context.Add(image);
                    await _context.SaveChangesAsync();

                    contest.ContestImageId = image.ImageId;
                }
                //set contest upload date
                contest.ContestUploadDate = DateTime.Now;
                contest.ContestEditDate = DateTime.Now;
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

            var contest = await _context.Contests.Include(c => c.ContestImage).Where(c => c.ContestId.Equals(id)).FirstOrDefaultAsync();
            if (contest == null)
            {
                return NotFound();
            }
            return View(contest);
        }

        // POST: Edit existing contest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContest(Guid id, 
            [Bind("ContestId,ContestTitle,ContestText,ContestStartDate,ContestEndDate,Email,ContestUploadDate,ContestImageId")] Contests contest, 
            IFormFile file, string contestStatus)
        {
            if (id != contest.ContestId)
            {
                return NotFound();
            }

            if (contestStatus == "EditContest")
            {
                //check if image is valid
                ValidImageUpload(file, "Contest");

                if (ModelState.IsValid)
                {
                    //add image to image folder if employee uploaded one
                    if (file != null)
                    {
                        Images image = new Images();
                        var filePath = Path.Combine("/Images/UploadedContent/" + file.FileName);
                        int counter = 1;

                        //check if image share name of other images
                        while (System.IO.File.Exists("wwwroot" + filePath))
                        {
                            //add incremented value
                            string newFilePath = file.FileName.Replace(".", $"({counter}).");
                            filePath = Path.Combine("/Images/UploadedContent/" + newFilePath);
                            counter++;
                        }
                        //add image to project
                        var stream = new FileStream("wwwroot" + filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        //add to database
                        image.ImageId = Guid.NewGuid();
                        image.ImagePath = filePath;
                        _context.Add(image);
                        await _context.SaveChangesAsync();

                        contest.ContestImageId = image.ImageId;
                    }
                    //set contest edit date
                    contest.ContestEditDate = DateTime.Now;
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
            }
            //Remove Image from Contest
            else
            {
                contest.ContestImageId = null;
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

        //image validation
        private bool ValidImageUpload (IFormFile file, string errorPrefix)
        {
            //return value
            bool validUpload = true;
            string errorField; 
            if(errorPrefix == "Dyk")
                errorField = $"{errorPrefix}image";
            else
                errorField = $"{errorPrefix}Image";


            //values to validate against
            List<string> fileextensions = new List<string> { ".jpeg", ".jpg", ".png", ".gif" };
            double limitSize = 1024 * 1024 * 5; //5MB file size
            if (file != null)
            {
                //get extension of file
                string ext = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).ToLower();

                //check file extensions
                if (!fileextensions.Contains((Path.GetExtension(file.FileName))))
                {
                    ModelState.AddModelError(errorField, "Invalid image file, must select a *.jpeg, *.jpg, *.gif, or *.png file.");
                    validUpload = false;
                }

                //check file size
                if (file.Length > limitSize)
                {
                    ModelState.AddModelError(errorField, "File is too big, please upload image with a size less than 5MB.");
                    validUpload = false;
                }
            }
            return validUpload;
        }

        /// <summary>
        /// Create selectlist with option to add new categories
        /// </summary>
        /// <returns></returns>
        private SelectList TTLCategoryList(Guid selectedValue)
        {
            Categories blank = new Categories()
            {
                CategoryId = Guid.Empty,
                CategoryName = "New"
            };
            List<Categories> categories = new List<Categories>(_context.Categories.OrderBy(c => c.CategoryName));
            categories.Add(blank);
            SelectList categorySelectList = new SelectList(categories, "CategoryId", "CategoryName", selectedValue);
            return categorySelectList;
        }

    }
}
