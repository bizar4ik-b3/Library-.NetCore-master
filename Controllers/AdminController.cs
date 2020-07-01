using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
    public class AdminController : Controller
    {
        IHostingEnvironment _appEnvironment;
        private DataContext db;

        public AdminController(DataContext context, IHostingEnvironment appEnvironment)
        {
            db = context;
            _appEnvironment = appEnvironment;
        }

[Authorize(Roles = "Publisher,Admin,Director,Manager")]
        public IActionResult Index()
        {
            ViewBag.TotalUsers=db.Users.Count();
            ViewBag.TotalDocuments=db.Documents.Count();
            ViewBag.TotalCategories=db.Categories.Count();
            return View();
        }


        [Authorize(Roles = "Publisher,Admin,Director,Manager")]
        public IActionResult AddFile()
        {
            List<Category> categories = db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Publisher,Admin,Director,Manager")]
        [HttpPost]
        public async Task<IActionResult> AddFile(AddFileModelView file)
        {
            if (file.uploadedFile != null)
            {
                string ext=Path.GetExtension(file.uploadedFile.FileName).ToLowerInvariant();
                var types=GetMimeTypes();
                string newName=DateTime.Now.ToString("M-d-yyyy_hh-mm-ss")+ext;
                string path ="";
                string MonthNumber=DateTime.Now.Month.ToString();
                string YearNumber=DateTime.Now.Year.ToString();

                DirectoryInfo directory=new DirectoryInfo(_appEnvironment.WebRootPath+@"\Files\"+YearNumber+@"\"+MonthNumber);
                if(directory.Exists)
                {
                   path = "\\Files\\" +YearNumber+@"\"+MonthNumber+@"\"+newName;
                }
                else
                {
                    directory.Create();
                    path = "\\Files\\" +YearNumber+@"\"+MonthNumber+@"\"+newName;

                }

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await file.uploadedFile.CopyToAsync(fileStream);
                }
                string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
                LibDocument  newfile = new LibDocument();

                switch (ext)
                {
                    case ".pdf":
                    

                    //PDFParser pDF = new PDFParser(_appEnvironment);
                   //Task<string> pdfText=Task.Factory.StartNew(()=>pDF.ReadPdfFile(path));
                        newfile = new LibDocument {
                        Name = file.Name,
                        Path = path,
                        Desc1 = file.Desc1,
                        UserId=Int32.Parse(userId),
                        CategoryId=file.categories,
                        DbName=newName,
                        Published=file.Published,
                        AccessLinkOnly=file.AccessLinkOnly,
                        ContentType=types[ext]
                            };
                    break;
                    
                    case ".mp4":
                        newfile = new LibDocument {
                        Name = file.Name,
                        Path = path,
                        Desc1 = file.Desc1,
                       UserId=Int32.Parse(userId),
                       CategoryId=file.categories,
                       DbName=newName,
                       Published=file.Published,
                       AccessLinkOnly=file.AccessLinkOnly,
                       ContentType=types[ext]
                        };
                    break;

                    case ".doc":
                    case ".docx":
                    DocxParser docx=new DocxParser(_appEnvironment);
                            newfile = new LibDocument {
                        Name = file.Name,
                        Path = path,
                        Desc1 = file.Desc1,
                        Desc2 = docx.GetText(path),
                        UserId=Int32.Parse(userId),
                        CategoryId=file.categories,
                        DbName=newName,
                        Published=file.Published,
                        AccessLinkOnly=file.AccessLinkOnly,
                        ContentType=types[ext]
                            };
                    break;
                }

                db.Documents.Add(newfile);
                await db.SaveChangesAsync();
                await Task.Factory.StartNew(() => InsertPdfTextToDbAsync(newfile.id, path));
                List<Category> categories = db.Categories.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }
            return RedirectToAction("MyFiles","File");
        }

        private  void InsertPdfTextToDbAsync(int id,string path)
        {
            var record=db.Documents.Where(rec=>rec.id==id).FirstOrDefault();
            PDFParser pDF = new PDFParser(_appEnvironment);
            Task<string> task=Task<string>.Factory.StartNew(pDF.ReadPdfFile, path);
            task.Wait();
            record.Desc2=task.Result;
            AddToIndexDocAsync(id);
            db.SaveChanges();
        }

       


        private async void AddToIndexDocAsync(int id)
        {
            var doc = await db.Documents.FindAsync(id);
            Search index = new Search();
            index.createIndex(doc);
        }

        [Authorize(Roles = "Publisher,Admin,Director,Manager")]
        public string Search(string field = "")
        {
           // string field1 = field.Replace("(!)", "");
            if (field != "")
            {
                Search search = new Search();
                var arr = search.searchfield(field);
                if (arr != null)
                {
                    return "Seacrh compleated";
                }
            }
            return "No records";
        }


[Authorize(Roles = "Publisher,Admin,Director")]
        public IActionResult ExecutorList()
        {
            var DocsToChecked=db.Documents
            .Where(x=>x.DocChecked==false)
            .Where(e=>e.Published==true)
            .Include(m=>m.Category)
            .Include(t=>t.User)
            .ToList();

            ViewBag.DocsToCheckedCount=DocsToChecked.Count();         

            return View(DocsToChecked);
        }

        [Authorize(Roles = "Publisher,Admin,Director")]
        [HttpPost]
            public IActionResult executorList(int docId, bool DocChecked)
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            var doc=db.Documents
            .Where(x=>x.id==docId).FirstOrDefault();
            doc.DocChecked=DocChecked;
            doc.ExecutorId=Int32.Parse(userId);
            db.SaveChanges();
            return RedirectToAction("ExecutorList");
        }
        
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".mp4","video/mp4"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
















    }
}