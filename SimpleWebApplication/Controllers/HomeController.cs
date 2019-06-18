using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleWebApplication.Models;

namespace SimpleWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<MyUser> userManager;

        public HomeController(UserManager<MyUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    user = new MyUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName
                    };
                }

                var result = await this.userManager.CreateAsync(user, model.Password1);

                return this.View("Success");
            }

            return this.View();
        }

        [HttpGet]
        public IActionResult Login([FromQuery]string returnTo)
        {
            this.ViewData["ReturnTo"] = returnTo;
            return this.View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, [FromQuery]string returnTo)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByNameAsync(model.UserName);
                if (user != null && await this.userManager.CheckPasswordAsync(user, model.Password))
                {
                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

                    await this.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
                    return this.Redirect(returnTo ?? this.Url.Action("Index", "Home"));
                }

                this.ModelState.AddModelError("", "Invalid UserName or Password");
            }

            return this.View();
        }

    }
}