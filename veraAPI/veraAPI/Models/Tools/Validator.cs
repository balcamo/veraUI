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
        private Scribe Log;

        public Validator(Scribe Log)
        {
            this.Log = Log;
        }

        public bool CompareAlphaBravo(object Alpha, object Bravo)
        {
            bool result = false;
            Type AlphaType = Alpha.GetType();
            Type BravoType = Bravo.GetType();
            Log.WriteLogEntry("Alpha type: " + AlphaType.FullName);
            Log.WriteLogEntry("Bravo type: " + BravoType.FullName);
            if (AlphaType == BravoType)
            {
                FieldInfo[] AlphaFields = AlphaType.GetFields();
                FieldInfo[] BravoFields = BravoType.GetFields();
                foreach (FieldInfo Field in AlphaFields)
                {
                    Log.WriteLogEntry("Alpha field value: " + Field.GetValue(Alpha));
                    Log.WriteLogEntry("Bravo field value: " + Field.GetValue(Bravo));
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
            return result;
        }
    }
}
