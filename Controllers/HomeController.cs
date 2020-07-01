using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
//using MySql.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using MySql.Data.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using System.Text.RegularExpressions;


namespace MvcMovie.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private DataContext db;

        public HomeController(DataContext context)
        {
            db = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.TotalDocuments = db.Documents
            .Where(x => x.DocChecked == true)
            .Where(e => e.Published == true)
            .Count();
            ViewBag.TotalCategories = db.Categories.Count();
            ViewBag.TotalMediaFiles = db.Documents
            .Where(x => x.ContentType == "video/mp4")
            .Count();

            return View();
        }

        static List<SearchDoc> Docs;
        static string SearchFlag { get; set; }
        [Route("Search")]
        public IActionResult Search(string searchQuery, int page = 1)
        {

            if ((searchQuery != null) && (Docs == null) || SearchFlag != searchQuery)
            {
                Search search = new Search();
                ViewBag.Query = SearchFlag = searchQuery;
                string searchQuery1 = Regex.Replace(searchQuery, @"((\b)[а-яА-ЯёЁa-zA-Z]{1,2}(\b))|[^\.\-\w\d\s]+/gmu", "");
                searchQuery1 = Regex.Replace(searchQuery1, @"(^[^\w\d]+)|(\W*$)", "");
                if (searchQuery1 != "")
                {
                    Docs = search.searchfield(searchQuery1);
                    foreach (var doc in Docs.ToList())
                    {
                        var c = db.Documents.Where(x => x.id == Int32.Parse(doc.id))
                        .Include(x => x.Category).FirstOrDefault();

                        doc.CategoryId = c.Category.Id;
                        doc.CategoryName = c.Category.Name;
                        doc.Desc2 = c.Desc2;
                        doc.Name = c.Name;
                        doc.Path = c.Path;
                        if ((c.Published == false) || (c.DocChecked == false) || (c.AccessLinkOnly == true))
                        {
                            Docs.Remove(doc);
                        }
                    }
                    ViewBag.Categories = db.Categories.ToList();
                    ViewBag.Count = Docs.Count();
                }
            }

            int pageSize = 10; // количество элементов на странице

            var count = Docs.Count();
            var items = Docs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            SearchViewModel viewModel = new SearchViewModel
            {
                PageViewModel = pageViewModel,
                SearchDocs = items,
            };
            ViewBag.Count = Docs.Count();
            ViewBag.Query = SearchFlag;
            ViewBag.SearchQuery = SearchFlag;
            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
