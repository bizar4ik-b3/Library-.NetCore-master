
using System.Linq;
using MvcMovie.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
namespace MvcMovie.Components
{
    public class MenuNewViewComponent
    {
        private  DataContext db;
        public bool userIsAdmin=false;
        public MenuNewViewComponent(DataContext dataContext)
        {
            db=dataContext;

        }
        public HtmlString Invoke()
        {
             return GetMenuItems();
        }

        public   HtmlString GetMenuItems()
        {    
            var  parentItems=db.Categories.Where(p=>p.parent_id==0).ToList();
            TagBuilder ul = new TagBuilder("ul");
           
            foreach(var item in parentItems)
            {
                 
                 TagBuilder li = new TagBuilder("li");
                 TagBuilder a = new TagBuilder("a");
                 TagBuilder i = new TagBuilder("i");
                 TagBuilder span = new TagBuilder("span");
                 TagBuilder strong = new TagBuilder("strong");
                 TagBuilder small = new TagBuilder("small");
                  var subChild=db.Categories.Where(c=>c.parent_id==item.Id).ToList();

                   a.Attributes.Add("href","/Category/Show/"+item.Id.ToString());
                   a.Attributes.Add("class","context");
                    a.Attributes.Add("id",item.Id.ToString());
                    i.Attributes.Add("class","fas fa-fw fa-folder");
                    strong.InnerHtml.AppendHtml(item.Name);
                    strong.Attributes.Add("style","text-transform:none");
                    
                    a.InnerHtml.AppendHtml(i);
                    a.InnerHtml.AppendHtml(strong);
                    
                    li.InnerHtml.AppendHtml(a);
                    ul.InnerHtml.AppendHtml(li);

                 if(subChild.Count>0)
                 {
                
                    li.InnerHtml.AppendHtml(AddChildItem(item,li).Replace("Microsoft.AspNetCore.Mvc.Rendering.TagBuilder","")); 
                 }
                
            }
            ul.Attributes.Add("class", "mcd-menu");
            
            ul.Attributes.Add( "oncontextmenu" ,@"event.preventDefault();$('#context-menu').show();$('#context-menu').offset({'top':mouseY,'left':mouseX})");

            var writer = new System.IO.StringWriter();
            ul.WriteTo(writer, HtmlEncoder.Default);
            
            return new HtmlString(writer.ToString());
           
        }

        private  string AddChildItem(Category childItem, TagBuilder pli)
            {
                
                TagBuilder ul = new TagBuilder("ul");
                ul.Attributes.Add("style","margin-left:50px");
        
                
               var childItems =db.Categories.Where(c=>c.parent_id==childItem.Id).ToList();
                foreach (var cItem in childItems)
                {
                        TagBuilder li = new TagBuilder("li");
                        TagBuilder a = new TagBuilder("a");
                        TagBuilder i = new TagBuilder("i");

                            a.Attributes.Add("href","/Category/Show/"+cItem.Id.ToString());
                            a.Attributes.Add("id",cItem.Id.ToString());
                            a.Attributes.Add("class","context");
                            i.Attributes.Add("class","fa fa-ellipsis-v");
                            a.InnerHtml.Append(cItem.Name);                  
                            a.InnerHtml.AppendHtml(i);
                            li.InnerHtml.AppendHtml(a);
                        
                        var subChild=db.Categories.Where(c=>c.parent_id==cItem.Id).ToList();
                        if (subChild.Count > 0)
                        {
                            AddChildItem(cItem, li);
                        }
                
                    ul.InnerHtml.AppendHtml(li);
                }
                pli.InnerHtml.AppendHtml(ul);
                return pli.ToString();
            }
        }
    }
