using System.ComponentModel.DataAnnotations;
using System.Text;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace AuthWeb.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
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
            [Display(Name = "Phone")]
            public string Phone { get; set; }


            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [Display(Name = "Image")]
            public IFormFile Image { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Phone = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

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
            var imagePath = await UploadImageToFirebaseStorage(Input.Image);

            using (var httpClient = new HttpClient())
            {
                var userdbResponse = await httpClient.GetAsync($"https://localhost:7293/api/User/{user.Email}");
                var userdbContent = await userdbResponse.Content.ReadAsStringAsync();
                var userdbObject = JsonConvert.DeserializeObject<dynamic>(userdbContent);
                var userData = new
                {
                    userId = userdbObject.userId,
                    username = Input.Username,
                    email = user.Email,
                    phone = Input.Phone,
                    password = user.PasswordHash,
                    imagePath = imagePath,
                    // Add any other properties you want to send
                };
                var jsonUserData = JsonConvert.SerializeObject(userData);
                var content = new StringContent(jsonUserData, Encoding.UTF8, "application/json");
                var apiResponse = await httpClient.PutAsync($"https://localhost:7293/api/User/{userdbObject.userId}", content);

            }
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.Phone != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.Phone);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
            private async Task<string> UploadImageToFirebaseStorage(IFormFile image)
            {
                var stream = image.OpenReadStream();
                var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCwkEYL7wncj6ih1XuBfrw-5oUM_90I2PM"));
                var cancellation = new CancellationTokenSource();

                var firebaseStorage = new FirebaseStorage(
                    "petpaws-6b2e3.appspot.com",
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true // Enables exception throwing on cancel operations
                    }
                );

                // Set a unique name for the image in Firebase Storage
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

                // Upload the image to Firebase Storage
                var imageUrl = await firebaseStorage
                    .Child("images")
                    .Child(imageName)
                    .PutAsync(stream, cancellation.Token);

                return imageUrl;
            }

        }
   
}
