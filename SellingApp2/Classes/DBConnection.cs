using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Configuration;

namespace SellingApp2
{
    public class DBConnection
    {
        public static string ConnectionString
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings["SellingConnectionString"].ConnectionString.ToString();
            }

           
        }
    }
}