﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace VeraAPI.Models
{
    public class Validator
    {
        public bool CompareAlphaBravo(object Alpha, object Bravo)
        {
            bool result = false;
            Type AlphaType = Alpha.GetType();
            FieldInfo[] AlphaFields = AlphaType.GetFields();
            Type BravoType = Bravo.GetType();
            FieldInfo[] BravoFields = BravoType.GetFields();
            if (AlphaType == BravoType)
            {
                result = AlphaFields.Intersect(BravoFields).Any();
            }
            return result;
        }
    }
}
