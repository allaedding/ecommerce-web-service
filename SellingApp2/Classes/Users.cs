using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace SellingApp2
{
    public class Users
    {
        public int IsAvailable(string UserName, string Email)
        {
            int UserID = 0;
            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users where Username= @Username or Email=@Email ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    connection.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.FieldCount > 0)
                    {
                        reader.Read();
                        UserID = reader.GetInt32(0);
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return UserID;
        }
    }
}
