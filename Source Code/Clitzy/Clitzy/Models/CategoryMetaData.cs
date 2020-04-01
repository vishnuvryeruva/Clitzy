using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clitzy.Models
{
    public class CategoryMetaData
    {
        [Required]
        public string Name { get; set; }

    }

    [MetadataType(typeof(CategoryMetaData))]
    public partial class Category
    {

    }

}