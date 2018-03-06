using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notestash_Admin.Models
{
    public class userData
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[] ProfilePicture { get; set; }
        public int Status { get; set; }
    }
}