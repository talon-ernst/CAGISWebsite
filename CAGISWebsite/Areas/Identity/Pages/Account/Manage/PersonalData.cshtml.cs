
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAGISWebsite.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        public IActionResult OnGet()
        {
            Response.StatusCode = 404;
            return Page();
        }
    }
}