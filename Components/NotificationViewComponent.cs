using System;
using System.Linq;
using MvcMovie.Models;
using Microsoft.EntityFrameworkCore;

namespace MvcMovie.Components
{
    public class NotificationViewComponent
    {
        private DataContext db;
        public bool userIsAdmin = false;
        public NotificationViewComponent(DataContext dataContext)
        {
            db = dataContext;
        }
        public string Invoke()
        {
            return GetDocsToExecute();
        }

        public String GetDocsToExecute()
        {
            var DocsToChecked = db.Documents
            .Where(x => x.DocChecked == false)
            .Where(e => e.Published == true)
            .Include(m => m.Category)
            .Include(t => t.User)
            .Count();

            return DocsToChecked.ToString();

        }
    }
}
