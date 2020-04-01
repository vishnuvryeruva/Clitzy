using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clitzy.Models
{
    public class SettingMetaData
    {
        [Required]
        public string Value { get; set; }
        
    }

    [MetadataType(typeof(SettingMetaData))]
    public partial class Setting
    {

    }

}