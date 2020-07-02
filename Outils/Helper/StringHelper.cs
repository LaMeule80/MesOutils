using System;
using System.Globalization;

namespace Outils.Helper
{
    public static class StringHelper
    {
        private static readonly string espace = @"&nbsp;";
        private static readonly string et = "&amp;";
        private static readonly string eAccent = "&eacute;;";
        private static readonly string apostrophe1 = "&#x27;";
        private static readonly string apostrophe2 = "&#039;";

        public static string ToTitleCase(string text)
        {
            var myTi = CultureInfo.CurrentCulture.TextInfo;
            return myTi.ToTitleCase(text.ToLower());
        }

        public static string SupprimeCaractereParasite(string innerText)
        {
            return innerText.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }

        public static string CorrigeLienInternet(string innerText)
        {
            return innerText
                .Replace(espace, " ")
                .Replace(et, "&")
                .Replace(eAccent, "é")
                .Replace(apostrophe1, "'")
                .Replace(apostrophe2, "'");
        }

        public static int DamerauLevenshteinDistance(string string1, string string2, int threshold)
        {
            // Return trivial case - where they are equal
            if (string1.Equals(string2))
                return 0;

            // Return trivial case - where one is empty
            if (string.IsNullOrEmpty(string1) || string.IsNullOrEmpty(string2))
                return string1.Length + (string2 ?? "").Length;

            // Ensure string2 (inner cycle) is longer
            if (string1.Length > string2.Length)
            {
                var tmp = string1;
                string1 = string2;
                string2 = tmp;
            }

            // Return trivial case - where string1 is contained within string2
            if (string2.Contains(string1))
                return string2.Length - string1.Length;

            var length1 = string1.Length;
            var length2 = string2.Length;

            var d = new int[length1 + 1, length2 + 1];

            for (var i = 0; i <= d.GetUpperBound(0); i++)
                d[i, 0] = i;

            for (var i = 0; i <= d.GetUpperBound(1); i++)
                d[0, i] = i;

            for (var i = 1; i <= d.GetUpperBound(0); i++)
            {
                var im1 = i - 1;
                var im2 = i - 2;
                var minDistance = threshold;

                for (var j = 1; j <= d.GetUpperBound(1); j++)
                {
                    var jm1 = j - 1;
                    var jm2 = j - 2;
                    var cost = string1[im1] == string2[jm1] ? 0 : 1;

                    var del = d[im1, j] + 1;
                    var ins = d[i, jm1] + 1;
                    var sub = d[im1, jm1] + cost;

                    //Math.Min is slower than native code
                    //d[i, j] = Math.Min(del, Math.Min(ins, sub));
                    d[i, j] = del <= ins && del <= sub ? del : ins <= sub ? ins : sub;

                    if (i > 1 && j > 1 && string1[im1] == string2[jm2] && string1[im2] == string2[jm1])
                        d[i, j] = Math.Min(d[i, j], d[im2, jm2] + cost);

                    if (d[i, j] < minDistance)
                        minDistance = d[i, j];
                }

                if (minDistance > threshold)
                    return int.MaxValue;
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)] > threshold
                ? int.MaxValue
                : d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }
    }
}