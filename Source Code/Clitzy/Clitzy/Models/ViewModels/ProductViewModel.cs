using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clitzy.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product product { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public SelectList CategoriesMultiLevel  { get; set; }
    }
}