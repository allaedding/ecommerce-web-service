using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace SellingApp2
{
    public class StringGeneration
    {
        public static string GetString()
        {
            string str = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");
            str = str.Substring(0, str.Length - 2);
            return str;
        }
    }
}