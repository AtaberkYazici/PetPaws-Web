using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AuthWeb.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Hosting.Server;
using Firebase.Auth;
using Firebase.Storage;

namespace AuthWeb.Controllers;

public class HomeController : Controller
{
    private static string Apikey = "AIzaSyCwkEYL7wncj6ih1XuBfrw-5oUM_90I2PM";
    private static string Bucket = "petpaws-6b2e3.appspot.com";
    private static string AuthEmail = "firebasestorage@gmail.com";
    private static string AuthPassword = "Ata.123";

    private readonly ApiGateway _apiGateway;

    public HomeController(ApiGateway apiGateway)
    {
        _apiGateway = apiGateway;
    }
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Main()
    {
        List<GetAnimalDto> animals;
        animals = _apiGateway.ListAnimal();
        return View(animals);
    }
    public IActionResult AdoptedPost()
    {
        List<GetAnimalDto> animals;
        string userEmail = User.Identity.Name;
        ViewBag.UserEmail = userEmail;
        animals = _apiGateway.SavedAnimal(userEmail);
        return View(animals);
    }
    public IActionResult MyPost()
    {
        List<GetAnimalDto> animals;
        string userEmail = User.Identity.Name;
        animals = _apiGateway.myAnimals(userEmail);
        return View(animals);
    }
    public IActionResult CreatePost(IFormFile file, string name, string type, int age, string details, string imagepath, string username, string[] vaccines)
    {
        string userEmail = User.Identity.Name;
        // Store the user's email in ViewBag
        ViewBag.UserEmail = userEmail;

        return View();
    }
    public IActionResult EditPost(int AnimalId)
    {
        string userEmail = User.Identity.Name;

        // Store the user's email in ViewBag
        ViewBag.UserEmail = userEmail;
        ViewBag.AnimalId = AnimalId;
        return View();
    }
    public IActionResult Animal(int AnimalId)
    {
        string userEmail = User.Identity.Name;
        var animal=_apiGateway.GetAnimalbyId(AnimalId);
        var user = _apiGateway.GetUserbyEmail(animal.userName);
        // Store the user's email in ViewBag
        ViewBag.UserEmail = userEmail;
        ViewBag.imagePath = user.imagePath;
        return View(animal);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task Upload(Stream stream, string fileName)
    {
        // ... (existing code)

        var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Apikey));
        var auth = await authProvider.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

        var cancellation = new CancellationTokenSource();

        var task = new FirebaseStorage(
            Bucket,
            new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                ThrowOnCancel = true
            })
            .Child("images")
            .Child(fileName)
            .PutAsync(stream, cancellation.Token);

        try
        {
            // error during upload will be thrown when await the task
            string link = await task;
            Console.WriteLine($"Image uploaded. Download link: {link}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception was thrown: {0}", ex);
        }

    }
}

