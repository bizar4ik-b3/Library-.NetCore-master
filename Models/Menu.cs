using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;



namespace MvcMovie.Models
{
    [Authorize]
    public class Menu : Controller
    {
        public bool userIsAdmin = false;

        public static DataContext db;


        public HtmlString GetMenuItems()
        {
            var parentItems = db.Categories.Where(p => p.parent_id == 0).ToList();
            TagBuilder ul = new TagBuilder("ul");

            foreach (var item in parentItems)
            {
                TagBuilder div = new TagBuilder("div");
                TagBuilder li = new TagBuilder("li");
                TagBuilder a = new TagBuilder("a");
                TagBuilder i = new TagBuilder("i");
                TagBuilder span = new TagBuilder("span");
                var subChild = db.Categories.Where(c => c.parent_id == item.Id).ToList();

                if (subChild.Count > 0)
                {
                    // li.InnerHtml.AppendHtml ("<a class=\"nav-link dropdown-toggle\" role=\"button\" data-toggle=\"dropdown\" href=\""+item.Id+"\"><i class=\"fas fa-fw fa-folder\"></i><span>"+item.Name+"</span></a>\"");
                    a.Attributes.Add("href", item.Id.ToString());
                    a.Attributes.Add("class", "nav-link dropdown-toggle");
                    a.Attributes.Add("role", "button");
                    a.Attributes.Add("data-toggle", "dropdown");
                    i.Attributes.Add("class", "fas fa-fw fa-folder");
                    a.InnerHtml.AppendHtml(i);
                    span.InnerHtml.Append(item.Name.ToString());
                    a.InnerHtml.AppendHtml(span);
                    li.InnerHtml.AppendHtml(a);

                }
                else
                {
                    // li.InnerHtml.AppendHtml ("<a class=\"nav-link\" role=\"button\" href=\""+item.Id+"\"><i class=\"fas fa-fw fa-folder\"></i><span>"+item.Name+"</span></a>\"");
                    a.Attributes.Add("href", item.Id.ToString());
                    a.Attributes.Add("class", "nav-link");
                    a.Attributes.Add("role", "button");
                    //a.Attributes.Add("data-toggle","dropdown");   
                    i.Attributes.Add("class", "fas fa-fw fa-folder");
                    a.InnerHtml.AppendHtml(i);
                    span.InnerHtml.Append(item.Name.ToString());
                    a.InnerHtml.AppendHtml(span);
                    li.InnerHtml.AppendHtml(a);
                }

                ul.InnerHtml.AppendHtml(li);

                if (subChild.Count > 0)

                    li.InnerHtml.AppendHtml(AddChildItem(item, div).Replace("Microsoft.AspNetCore.Mvc.Rendering.TagBuilder", ""));
                li.Attributes.Add("class", "nav-item dropdown");

                li.InnerHtml.AppendHtml(div);
                div.Attributes.Add("class", "dropdown-menu");
                ///////////////////////////////////////////////

                if (userIsAdmin)
                {
                    TagBuilder newa = new TagBuilder("a");
                    TagBuilder newa1 = new TagBuilder("a");

                    newa.Attributes.Add("href", "/Category/DeleteCategory/" + item.Id.ToString());
                    newa.Attributes.Add("style", "display: inline-block");
                    newa.InnerHtml.AppendHtml("X");

                    newa1.Attributes.Add("href", "/Category/EditCategory/" + item.Id.ToString());
                    newa1.Attributes.Add("style", "display: inline-block;margin-right:5px");
                    newa1.InnerHtml.AppendHtml("E");

                    li.InnerHtml.AppendHtml(newa);

                    li.InnerHtml.AppendHtml(newa1);

                }
            }
            ul.Attributes.Add("class", "sidebar navbar-nav");
            ul.Attributes.Add("style", "min-height:0");
            var writer = new System.IO.StringWriter();
            ul.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());

        }

        private string AddChildItem(Category childItem, TagBuilder pdiv)
        {

            var childItems = db.Categories.Where(c => c.parent_id == childItem.Id).ToList();
            foreach (var cItem in childItems)
            {
                TagBuilder div = new TagBuilder("div");
                TagBuilder a = new TagBuilder("a");
                a.InnerHtml.AppendHtml(cItem.Name);
                a.Attributes.Add("href", "/Category/Edit/" + cItem.Id.ToString());
                a.Attributes.Add("class", "dropdown-item btn popovers");
                a.Attributes.Add("style", "width:75%");

                if (userIsAdmin)
                {
                    TagBuilder newa = new TagBuilder("a");
                    TagBuilder newa1 = new TagBuilder("a");

                    newa.Attributes.Add("href", "/Category/DeleteCategory/" + cItem.Id.ToString());
                    newa.Attributes.Add("style", "display: inline-block");
                    newa.InnerHtml.AppendHtml("X");

                    newa1.Attributes.Add("href", "/Category/EditCategory/" + cItem.Id.ToString());
                    newa1.Attributes.Add("style", "display: inline-block;margin-right:5px");
                    newa1.InnerHtml.AppendHtml("E");

                    div.InnerHtml.AppendHtml(a);

                    div.InnerHtml.AppendHtml(newa);

                    div.InnerHtml.AppendHtml(newa1);

                    div.Attributes.Add("class", "d-flex flex-row");


                    pdiv.InnerHtml.AppendHtml(div);
                }
                else
                {
                    pdiv.InnerHtml.AppendHtml(a);
                }

            }
            return pdiv.ToString();

        }
    }

}

