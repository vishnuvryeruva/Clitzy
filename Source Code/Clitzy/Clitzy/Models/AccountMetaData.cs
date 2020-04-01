using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clitzy.Models
{
    public class AccountMetaData
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    [MetadataType(typeof(AccountMetaData))]
    public partial class Account
    {

    }

}