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

        public Validator(object alpha, object bravo)
        {
            this.Alpha = alpha;
            this.Bravo = bravo;
        }

        public bool CompareAlphaBravo()
        {
            bool result = false;
            Type AlphaType = Alpha.GetType();
            Type BravoType = Bravo.GetType();
            if (AlphaType == BravoType)
            {
                FieldInfo[] AlphaFields = AlphaType.GetFields();
                FieldInfo[] BravoFields = BravoType.GetFields();
                foreach (FieldInfo Field in AlphaFields)
                {
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
