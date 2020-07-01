using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MvcMovie.Models;
 
namespace MvcMovie.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PageViewModel PageModel { get; set; }
        
        public SearchViewModel Data { get; set; }
        public string PageAction { get; set; }

        
        public string SearchQuery { get; set; }
         
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";
 
            // набор ссылок будет представлять список ul
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");
 
            // // формируем три ссылки - на текущую, предыдущую и следующую
            // TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);
 
            // // создаем ссылку на предыдущую страницу, если она есть
            // if (PageModel.HasPreviousPage)
            // {
            //     TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
            //     tag.InnerHtml.AppendHtml(prevItem);
            // }
             
            // tag.InnerHtml.AppendHtml(currentItem);
            // // создаем ссылку на следующую страницу, если она есть
            // if (PageModel.HasNextPage)
            // {
            //     TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
            //     tag.InnerHtml.AppendHtml(nextItem);
            // }
            //формируем ссылки на все страницы
        for (int i = 1; i <= PageModel.TotalPages; i++)
        {
        TagBuilder currentItem = CreateTag(i, urlHelper);
        tag.InnerHtml.AppendHtml(currentItem);
        }

            output.Content.AppendHtml(tag);
        }
 
        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == this.PageModel.PageNumber)
            {
                item.AddCssClass("page-item");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(PageAction, new {SearchQuery=SearchQuery, page = pageNumber });
            }
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}