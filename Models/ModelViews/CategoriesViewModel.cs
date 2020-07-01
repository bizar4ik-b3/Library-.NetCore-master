using System.Collections.Generic;
 
namespace MvcMovie.Models
{
    public class CategoriesViewModel
    {
        public IEnumerable<LibDocument> Documents { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}