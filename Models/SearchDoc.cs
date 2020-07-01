using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class SearchDoc
    {
        public string id { get; set; }
        public string Name { get; set; }

        public string Desc2 { get; set; }
        public string Desc1 { get; set; }

        public string Path { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Category Category { get; set; }

    }
}