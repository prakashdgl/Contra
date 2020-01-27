using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Contra.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Contra.Models;
using Contra.Data;

namespace Contra.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ContraUser> _userManager;
        private readonly SignInManager<ContraUser> _signInManager;

        public IndexModel(ApplicationDbContext context,
                          UserManager<ContraUser> userManager,
                          SignInManager<ContraUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "A short summary of yourself")]
            public string Bio { get; set; }

            [DataType(DataType.Upload)]
            [Display(Name = "Profile picture file")]
            public IFormFile Image { get; set; }
        }

        private async Task LoadAsync(ContraUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Name = user.Name,
                Bio = user.Bio
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (string.IsNullOrEmpty(user.ProfilePictureURL)) user.ProfilePictureURL = "";
            await _userManager.UpdateAsync(user);

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }

            if (Input.Bio != user.Bio)
            {
                user.Bio = Input.Bio;
            }

            if (Input.Image != null)
            {
                string name = Path.GetRandomFileName();

                Image i = new Image
                {
                    OwnerID = user.Id,
                    Name = "pfp/" + name,
                    ContentType = Input.Image.ContentType,
                };

                using (var rs = Input.Image.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    rs.CopyTo(ms);
                    i.Content = ms.ToArray();
                }

                _context.Image.Add(i);
                await _context.SaveChangesAsync();

                user.ProfilePictureURL = "/img/upload/pfp/" + name;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
