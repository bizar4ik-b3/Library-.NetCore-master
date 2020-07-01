
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.Encodings;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace MvcMovie.Controllers
{
    [Authorize(Roles = "Admin,Director")]
    public class UserController : Controller
    {
        private IHostingEnvironment _appEnvironment;
        private DataContext db;

        public UserController(DataContext context, IHostingEnvironment appEnvironment)
        {
            db = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            List<User> users;
            if (User.IsInRole("Director"))
            {
                users = db.Users.Include(x => x.Roles).ToList();
            }
            else
            {
                users = db.Users.Include(x => x.Roles).Where(r => r.Roles.name != "Director").ToList();
            }

            ViewBag.UsersCount = users.Count();
            return View(users);
        }

        public IActionResult AddUser()
        {

            var roles = db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "id", "name");

            return View();
        }


        [HttpPost]
        public IActionResult addUser(User user)
        {

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                UserRole ur = new UserRole();
                ur.RoleId = user.Rolesid;
                ur.UserId = user.id;
                db.UserRoles.Add(ur);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            var roles = db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "id", "name");
            return View(user);
        }

        public IActionResult Delete(int id)
        {
            var docs = db.Documents.Where(fs => fs.UserId == id).ToList();
            var user = db.Users.Find(id);
            foreach (var doc in docs)
            {

                DeleteDoc(doc);


            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void DeleteDoc(LibDocument doc)
        {
            DirectoryInfo di = new DirectoryInfo(_appEnvironment.WebRootPath + doc.Path);

            Search index = new Search();
            index.DeleteFromIndex(doc);

            db.Documents.Remove(doc);
            db.SaveChanges();

        }


        public IActionResult Edit(int id)
        {
            User user = db.Users.Include(s => s.Roles).SingleOrDefault(x => x.id == id);
            var roles = db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "id", "name");

            return View(user);
        }

        [HttpPost]
        public IActionResult edit(User user)
        {
            User ExUser = db.Users.Where(x => x.id == user.id).FirstOrDefault();
            ExUser.Rolesid = user.Roles.id;
            ExUser.name = user.name;
            ExUser.surName = user.surName;
            ExUser.middleName = user.middleName;
            ExUser.login = user.login;
            ExUser.password = user.password;
            ExUser.Desc = user.Desc;

            var roles = db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "id", "name");

            db.Entry(ExUser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit");
        }
    }
}