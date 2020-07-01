using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MvcMovie.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Html;


namespace MvcMovie.Controllers
{
    public class AccountController : Controller
    {
        private DataContext db;
        public AccountController(DataContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.login == model.login && u.password == model.LoginPassword); //Change Password to string!!!!
                    
                if (user != null)
                {
                    await Authenticate(user); // аутентификация
 
                    return RedirectToAction("Index", "Home");
                }
                 else { ModelState.AddModelError("", "Введені не вірні логін та(або) пароль");}
               
            }
           
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Roles?.name),
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString(), ClaimValueTypes.String)
            };
            
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
 
 
        public async Task<IActionResult> Logout()
        {
             if(User.Identity.IsAuthenticated)
            {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}