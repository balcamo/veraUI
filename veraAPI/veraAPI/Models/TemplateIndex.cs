using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace veraAPI.Models
{
    public class TemplateIndex
    {
        static readonly int InsertTravelAuth;
        static readonly int InsertTravelRecap;

        static TemplateIndex()
        {
            InsertTravelAuth = 1;
            InsertTravelRecap = 2;
        }
    }
}