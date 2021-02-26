using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CAGISWebsite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CAGISWebsite.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly CAGISKidsContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            CAGISKidsContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public partial class InputModel
        {
            [Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The passwords do not match")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.UserName, Email = Input.Email, EmailConfirmed = false};
                var employee = new Employees { Username = Input.UserName, AdminId = Guid.Parse(user.Id), IsActivated = true};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Employee");
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ListEmployees", "Administration");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public partial class InputModel : IValidatableObject
        {
            CAGISKidsContext _CAGISContext = new CAGISKidsContext();

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (UserName == null || UserName.Trim() == "")
                {
                    yield return new ValidationResult("User Name cannot be empty", new[] { nameof(UserName) });
                }
                else
                {
                    UserName = UserName.Trim();
                    Employees employee = _CAGISContext.Employees.Where(e => e.Username == UserName).FirstOrDefault();
                    if (employee != null)
                    {
                        yield return new ValidationResult("Username already in use", new[] { nameof(UserName) });
                    }
                }

                if (Email == null || Email.Trim() == "")
                {
                    yield return new ValidationResult("Email cannot be empty", new[] { nameof(Email) });
                }
                else
                {
                    Email = Email.Trim();
                    if (!TTLEmailValidation(Email))
                    {
                        yield return new ValidationResult("Email is not in the right format. EX. JohnSmith@CAGIS.ca", new[] { nameof(Email) });
                    }
                    else
                    {
                        AspNetUsers user = _CAGISContext.AspNetUsers.Where(u => u.Email == Email).FirstOrDefault();
                        if (user != null)
                        {
                            yield return new ValidationResult("Email already in use", new[] { nameof(Email) });
                        }
                    }
                }


                if (Password != null && Password.Trim() != "")
                {
                    if (!TTLStrongPassword(Password))
                    {
                        yield return new ValidationResult("Password must have at least one capital letter, one number, and one symbol(!@#$&*).", new[] { nameof(Password) });

                    }
                }

                yield return ValidationResult.Success;
            }
        }

        /// <summary>
        /// Checks if the input string is an email address.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool TTLEmailValidation(string input)
        {
            if (input == null || input == "")
            {
                return true;
            }
            Regex emailCheck = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (emailCheck.IsMatch(input))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the input string is a strong password.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool TTLStrongPassword(string input)
        {
            if (input == null || input == "")
            {
                return true;
            }
            //regex checks for 1 or more capitals, 1 or more symbols, and 1 or more numbers
            Regex passwordCheck = new Regex(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9]).{6,}$");
            if (passwordCheck.IsMatch(input))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
