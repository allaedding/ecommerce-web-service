using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace SellingApp2
{
    public class Tools
    {
        
            public String GetLastToolAdded(String UserID, String DateAdded)
            {
                String ToolID = "0";
                try
                {
                    SqlDataReader reader;
                    using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("SELECT ToolID FROM Tools where UserID=@UserID and DateAdd=@DateAdded");
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@DateAdded", DateAdded);
                        connection.Open();
                      
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ToolID = Convert.ToString(reader.GetInt64(0));
                        }

                        reader.Close();
                        connection.Close();

                    }

                }
                catch (Exception e)
                {

                }
                return ToolID;
            }
        }
    }
