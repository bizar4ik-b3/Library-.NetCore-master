using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MvcMovie.Controllers
{
    [AllowAnonymous]
    public class GCDNUserController : Controller
    {
        IHostingEnvironment _appEnvironment;
        public DataContext db;

        public GCDNUserController(IHostingEnvironment appEnvironment, DataContext context)
        {
            _appEnvironment = appEnvironment;
            db = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> redirect(GCDNuserView user)
        {
            if (user != null)
            {
                var c = Json(user);
                await Authenticate(user);
                if (user.DocumentLink != 0)
                {
                    return RedirectToAction("Show", new { id = user.DocumentLink });

                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<RedirectToActionResult> redirectText(GCDNuserView user)
        {
            if (user != null)
            {
                await Authenticate(user);
                if (user.DocumentLinkString != null)
                {
                    Regex regex = new Regex(@"([0-9]+)$\/?");
                    MatchCollection matches = regex.Matches(user.DocumentLinkString);

                    if ((matches.Count != 0) && (matches[0].Value != ""))
                    {
                        return RedirectToAction("Show", new { id = Convert.ToInt32(matches[0].Value) });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public IActionResult Show(int id)
        {
            bool identity;
            RedirectResult redirectResult;
            identity = User.Identity.IsAuthenticated;
            if (identity)
            {
                var doc = db.Documents.Where(x => x.id == id).FirstOrDefault();
                if (doc != null)
                {
                    redirectResult = new RedirectResult(doc.Path, true);
                }
                else
                {
                    redirectResult = new RedirectResult("/");
                }
            }
            else redirectResult = new RedirectResult("/", true);
            return redirectResult;
        }


        private async Task Authenticate(GCDNuserView user)
        {
            var claims = new List<Claim>();
            if (user.Role == null)
            {
                claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                        //new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String)
                    };
            }
            else
            {
                claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String)
                    };
            }
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public void test()
        {
            GCDNuserView user = new GCDNuserView();
            user.Name = "PEtr";
            user.Permisions = "Read";
            user.Id = 1056;
            user.DocumentLink = 45;
            user.Role = "Admin";
            user.Desc1 = "asdasdasdasd";
            string url = "http://localhost:5000/gcdnduser/redirect/";
            var json = JsonConvert.SerializeObject(user);
            HttpClient client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                var products = result.Content.ReadAsStringAsync().Result;
            }
        }

        [Route("Api/auth")]
        [HttpPost]
        [AllowAnonymous]
        public async void redirectTextJson([FromBody] GCDNuserView user)
        {
            string referer = Request.Headers["Referer"].ToString();
            if (user != null)
            {
                await Authenticate(user);
            }
        }

        [Route("Api/logout")]
        [AllowAnonymous]
        public async Task<IActionResult> logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return Ok();
        }
    }
}

