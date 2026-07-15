using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ELMS.Helpers
{
    public static class LoginValidationHelper
    {
        public static bool IsValidUserName(string userName)
        {
            try
            {
                string pattern = @"^[a-zA-z]+[_][a-zA-z]+[_][0-9]+$";
                Regex rg = new Regex(pattern);
                return rg.IsMatch(userName);
            }
            catch
            {
                return false;
            }
        }
    }
}
