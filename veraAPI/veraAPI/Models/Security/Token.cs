using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class Token
    {
        public string Header { get; set; }
        public string Payload { get; set; }
        public string Signature { get; set; }
        public string Secret { get; set; }

        public string UserEmail { get; private set; }
        public string UserType { get; private set; }


    }
}