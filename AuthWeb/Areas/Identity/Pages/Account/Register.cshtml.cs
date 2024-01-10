using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AuthWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

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

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var imagePath = await UploadImageToFirebaseStorage(Input.Image);
                    var userData = new
                    {
                        userId= 0,
                    username= Input.Username,
                    email= user.Email,
                    phone= Input.Phone,
                    password= user.PasswordHash,
                    imagePath= imagePath,
                        // Add any other properties you want to send
                    };
                    var jsonUserData = JsonConvert.SerializeObject(userData);
                    using (var httpClient = new HttpClient())
                    {
                        var content = new StringContent(jsonUserData, Encoding.UTF8, "application/json");
                        var apiResponse = await httpClient.PostAsync("https://localhost:7293/api/User", content);

                        if (apiResponse.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("User created a new account with password and data sent to API.");
                        }
                        else
                        {
                            _logger.LogError($"Failed to send data to API. StatusCode: {apiResponse.StatusCode}");
                        }
                    }



                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
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
