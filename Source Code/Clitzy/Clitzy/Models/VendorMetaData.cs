using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clitzy.Models
{
    public class VendorMetaData
    {
        
        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }

    [MetadataType(typeof(VendorMetaData))]
    public partial class Vendor
    {

    }

}