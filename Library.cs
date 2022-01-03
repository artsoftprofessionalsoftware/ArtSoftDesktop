using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtSoftDesktop
{
    public static class StringExtensions
    {
        public static bool IsNotEmpty(this string s)
        {
            return (s != null && s != string.Empty);
        }

        public static string Max(this string s, int m)
        {
            string r = "";
            s = s ?? "";
            if (s.Length > m)
            {
                r = s.Substring(0, m);
            }
            else
            {
                r = s;
            }
            return r;
        }

        public static string FillStart(this string s, int n)
        {
            return FillStart(s, ' ', n);
        }

        public static string FillStart(this string s, char c, int n)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n - s.Length; i++)
            {
                sb.Append(c);
            }
            string r = sb.Append(s).ToString();
            return r;
        }

        public static string FillEnd(this string s, int n)
        {
            return FillEnd(s, ' ', n);
        }

        public static string FillEnd(this string s, char c, int n)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(s);
            for (int i = 0; i < n - s.Length; i++)
            {
                sb.Append(c);
            }
            string r = sb.ToString();
            return r;
        }
    }
}
