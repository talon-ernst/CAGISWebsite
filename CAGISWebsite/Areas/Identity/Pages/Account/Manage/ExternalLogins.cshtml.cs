
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAGISWebsite.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        public IActionResult OnGet()
        {
            Response.StatusCode = 404;
            return Page();
        }
    }
}
