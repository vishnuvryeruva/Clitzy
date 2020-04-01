using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clitzy.Models
{
    public class ProductMetaData
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string Description { get; set; }
        
    }

    [MetadataType(typeof(ProductMetaData))]
    public partial class Product
    {

    }

}