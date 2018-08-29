using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Forms
{
    public class TokenForm
    {
        public string UserEmail { get; set; }
        public string SessionKey { get; set; }
        public int UserType { get; set; }
    }
}