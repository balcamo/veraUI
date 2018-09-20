using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace VeraAPI.Models.Tools
{
    public class Validator
    {
        private object Alpha;
        private object Bravo;
        private Scribe log;

        public Validator(object alpha, object bravo)
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "Validator_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            this.Alpha = alpha;
            this.Bravo = bravo;
        }

        public bool CompareAlphaBravo()
        {
            log.WriteLogEntry("Starting CompareAlphaBravo...");
            bool result = false;
            Type AlphaType = Alpha.GetType();
            Type BravoType = Bravo.GetType();
            if (AlphaType == BravoType)
            {
                FieldInfo[] AlphaFields = AlphaType.GetFields();
                FieldInfo[] BravoFields = BravoType.GetFields();
                foreach (FieldInfo Field in AlphaFields)
                {
                    log.WriteLogEntry("Compare values " + Field.GetValue(Alpha) + " " + Field.GetValue(Bravo));
                    if (object.Equals(Field.GetValue(Alpha), Field.GetValue(Bravo)))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        break;
                    }

                }
            }
            else
                result = false;
            log.WriteLogEntry("Compare Result " + result);
            log.WriteLogEntry("End CompareAlphaBravo.");
            return result;
        }
    }
}
