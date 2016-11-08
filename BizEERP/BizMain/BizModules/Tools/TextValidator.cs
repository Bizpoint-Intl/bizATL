using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Created By Jason
/// Used for Text Validations
/// </summary>

namespace ATL.BizModules.TextValidator
{
    class TextValidator
    {
        static public bool IsvalidName(string input)
        {
            string NameExpression = @"^[a-z -']+$";

            MatchCollection m1 = Regex.Matches(input, NameExpression);

            if (m1.Count > 0)
            {
                m1 = null;
                return true;
            }
            else
            {
                m1 = null;
                return false;
            }

        }

        static public bool IsvalidAddress(string input)
        {
            string AddressExpression = @"^[a-zA-Z\d]+(([\'\,\.\- #][a-zA-Z\d ])?[a-zA-Z\d]*[\.]*)*$";

            MatchCollection m2 = Regex.Matches(input, AddressExpression);

            if (m2.Count > 0)
            {
                m2 = null;
                return true;
            }
            else
            {
                m2 = null;
                return false;
            }

        }

        static public bool IsvalidTel(string input)
        {
            string TelExpression = "";

            MatchCollection m3 = Regex.Matches(input, TelExpression);

            if (m3.Count > 0)
            {
                m3 = null;
                return true;
            }
            else
            {
                m3 = null;
                return false;             
            }
        }

        static public bool IsvalidPostalCode(string input)
        {
            string PostExpression = "";

            MatchCollection m4 = Regex.Matches(input, PostExpression);

            if (m4.Count > 0)
            {
                m4 = null;
                return true;
                
            }
            else
            {
                m4 = null;
                return false;
            }
        }

        static public bool IsvalidMilitaryTime(string input)
        {
            string PostExpression = @"^([0-1][0-9]|[2][0-3])([0-5][0-9])$";

            MatchCollection m5 = Regex.Matches(input, PostExpression);

            if (m5.Count > 0)
            {
                m5 = null;
                return true;
            }
            else
            {
                m5 = null;
                return false;
            }
        }




        static public bool IsvalidDecimal(string input)
        {
            string PostExpression = @"(?!^0*$)(?!^0*\.0*$)^\d{1,18}(\.\d{1,2})?$";

            MatchCollection m6 = Regex.Matches(input, PostExpression);

            if (m6.Count > 0)
            {
                m6 = null;
                return true;
            }
            else
            {
                m6 = null;
                return false;
            }
        }

        static public bool Isvalid24hrTime(string input)
        {
            string PostExpression = @"^([0-1][0-9]|[2][0-3]):([0-5][0-9]):([0-5][0-9])$";

            MatchCollection m7 = Regex.Matches(input, PostExpression);

            if (m7.Count > 0)
            {
                m7 = null;
                return true;
            }
            else
            {
                m7 = null;
                return false;
            }
        }


        

    }
}
    

