using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Contra.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace Contra.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ContraUser> _userManager;

        public RegisterConfirmationModel(UserManager<ContraUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null) return RedirectToPage("/Index");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound($"Unable to load user with email '{email}'.");

            return Page();
        }
    }
}
