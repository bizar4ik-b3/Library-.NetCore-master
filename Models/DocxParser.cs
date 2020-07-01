using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SautinSoft.Document;
using Paragr = SautinSoft.Document.Paragraph;


namespace MvcMovie.Models
{
    public class DocxParser
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public DocxParser(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetText(string fileName)
        {

            var filename = _hostingEnvironment.WebRootPath + fileName;

            FileInfo pathToDocx = new FileInfo(filename);

            // Let's parse docx docuemnt and get all text from it. 
            DocumentCore docx = DocumentCore.Load(pathToDocx.FullName);

            StringBuilder text = new StringBuilder();

            foreach (Paragr par in docx.GetChildElements(true, ElementType.Paragraph))
            {
                foreach (Run run in par.GetChildElements(true, ElementType.Run))
                {
                    text.Append(run.Text);
                }
                text.AppendLine();
            }

            return text.ToString();
        }
    }
}