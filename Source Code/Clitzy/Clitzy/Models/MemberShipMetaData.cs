using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Clitzy.Models
{
    public class MemberShipMetaData
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Month { get; set; }

    }

    [MetadataType(typeof(MemberShipMetaData))]
    public partial class MemberShip
    {

    }

}