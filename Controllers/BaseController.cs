using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using Microsoft.AspNetCore.Html;

namespace MvcMovie.Controllers
{
    public class BaseController:Controller
    {
        public DataContext db;

        public BaseController(DataContext context)
        {
            db=context;
            Menu Menu = new Menu();
            Menu.db = db;
            HtmlString MenuModule = Menu.GetMenuItems();
            ViewBag.Menu = MenuModule;
        }
    }
}