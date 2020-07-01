using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace MvcMovie.Models
{
 

    public class GCDNuserView
    {
        public int Id { get; set; }
  
        public string Name { get; set; }

        public string Role { get; set; }

        public string Permisions { get; set; }

        public string Desc1 { get; set; }

        public int DocumentLink { get; set; }

        public string DocumentLinkString { get; set; }

    }
}

