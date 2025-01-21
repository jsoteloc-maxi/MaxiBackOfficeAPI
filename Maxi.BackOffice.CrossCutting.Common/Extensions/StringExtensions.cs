using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// valida si 2 string son iguales ignorando case
        /// </summary>
        public static bool EqualText(this string str, string b)
        {
            //con ignorecase
            return str.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space
        /// characters.
        /// </summary>
        public static bool IsBlank(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

    }
}
