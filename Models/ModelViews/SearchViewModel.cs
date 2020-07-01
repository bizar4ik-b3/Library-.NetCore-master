using System.Collections.Generic;
 
namespace MvcMovie.Models
{
    public class SearchViewModel
    {
        public IEnumerable<SearchDoc> SearchDocs { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public string SearchQuery { get; set; }
    }
}