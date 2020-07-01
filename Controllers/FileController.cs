using System;
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
    
    public class FileController:Controller
    {
          private  IHostingEnvironment _appEnvironment;
        private  DataContext db;
         
        public FileController(DataContext dataContext, IHostingEnvironment appEnvironment)
        {
            db=dataContext;
            _appEnvironment = appEnvironment;

        }

        [Authorize(Roles = "Admin, Publisher, Director, Manager")]
        public IActionResult MyFiles()
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            int user_id=Int32.Parse(userId);
            var MyFiles=db.Documents.Where(f=>f.UserId==user_id).Include(x=>x.Category).ToList();
            
            return View(MyFiles);
        }

       [Authorize(Roles = "Admin, Publisher, Director, Manager, User")]
        public IActionResult Show(int id)
        {
            RedirectResult redirectResult=new RedirectResult("/");
            if(User.Identity.IsAuthenticated)
            {
                var doc=db.Documents.Where(x=>x.id==id).FirstOrDefault();
             
                if(doc!=null)
                {
                    //redirectResult = new RedirectResult(doc.Path);
                    return GetFile(doc);
                }
            }
            else 
            {
                redirectResult=new RedirectResult("/");
            }
            string s=DateTime.Now.ToString("yyyy-MM-dd-h-mm-ss-tt");
         
        return redirectResult;
        }

        public FileResult GetFile(LibDocument doc)
        {
            
            string ReportURL = _appEnvironment.WebRootPath+doc.Path;  
            if(doc.ContentType=="application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                return PhysicalFile(ReportURL, doc.ContentType, doc.DbName); 
            }
            else
            {
                return PhysicalFile(ReportURL, doc.ContentType); 
            }
        }


        
        [Authorize(Roles = "Admin, Publisher, Director, Manager")]
        [HttpPost]
        public IActionResult edit(LibDocument doc)
        {
               if (ModelState.IsValid)
            {
                var editedDoc=db.Documents.Where(x=>x.id==doc.id).FirstOrDefault();
                editedDoc.Name=doc.Name;
                editedDoc.CategoryId=doc.CategoryId;
                editedDoc.Desc1=doc.Desc1;
                editedDoc.Published=doc.Published;
                editedDoc.AccessLinkOnly=doc.AccessLinkOnly;

            db.Documents.Attach(editedDoc);
            db.Entry(editedDoc).State = EntityState.Modified;
            
            db.SaveChanges();
            }
            return RedirectToAction("MyFiles");
            
        }

        [Authorize(Roles = "Admin, Publisher, Director, Manager")]
        public IActionResult Edit(int id)
        {
            var doc=db.Documents.Where(x=>x.id==id).FirstOrDefault();
            
             List<Category> categories = db.Categories.ToList();
             ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(doc);
        }

       [Authorize(Roles = "Admin, Publisher, Director, Manager")]
       public  IActionResult Delete(int id)
       {
           LibDocument doc=db.Documents.Find(id);
           System.IO.FileInfo di = new FileInfo(_appEnvironment.WebRootPath+doc.Path);
           
           string webRootPath = _appEnvironment.WebRootPath;
            string contentRootPath = _appEnvironment.ContentRootPath;
         
            if(di.Exists)
            {
            di.Delete();
            }
            
            Search index = new Search();
            index.DeleteFromIndex(doc);

           db.Documents.Remove(doc);
           db.SaveChanges();      
           return RedirectToAction("MyFiles");
       }

       [Authorize(Roles = "Admin, Publisher, Director, Manager")]
       public IActionResult Details(int id)
       {
           LibDocument doc=db.Documents.Where(x=>x.id==id).Include(r=>r.Executor).Include(e=>e.User).FirstOrDefault();
           return PartialView("_GetDetails",doc);
       }
    }
}