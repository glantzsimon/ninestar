using System.Collections.Generic;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class BlogViewModel
    {
        public List<Article> Articles { get; set; }
        public List<Tag> Tags { get; set; }
    }
}