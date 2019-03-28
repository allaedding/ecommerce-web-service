using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace SellingApp2    /// <summary>

{
    /// Summary description for UsersWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class UsersWS : System.Web.Services.WebService
    {


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void Register(String UserName, String Password, String Email, String PhoneNumber, String Logtit, String Latitle)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 1;
            String Message = "";
            Users myUsers = new Users();
            if (myUsers.IsAvailable(UserName, Email) == 0)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO Users(UserName,Password,Email,PhoneNumber,Logtit,Latitle) VALUES (@UserName,@Password,@Email,@PhoneNumber,@Logtit,@Latitle)");
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        cmd.Parameters.AddWithValue("@Logtit", Logtit);
                        cmd.Parameters.AddWithValue("@Latitle", Latitle);   
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                    }
                    Message = "Your account is created succefully";

                }
                catch (Exception ex)
                {
                    IsAdded = 0;
                    Message = ex.Message;

                }
            }
            else
            {
                IsAdded = 0;
                Message = "Username Or Email Is Reserved";
            }
            var JsonData = new
            {
                IsAdded = IsAdded,
                Message = Message
            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void Login(String UserName, String Password)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int UserID = 0;
            String Message = "";

            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users where Username=@UserName and Password=@Password");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);

                    connection.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        UserID = reader.GetInt32(0);
                    }
                    if (UserID == 0)
                    {
                        Message = "UserName or Password is incorrect";
                    }
                    reader.Close();

                    connection.Close();

                }
            }
            catch (Exception e)
            {
                Message = e.Message;

            }
            var JsonData = new
            {
                UserID = UserID,
                Message = Message
            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }




        [WebMethod(MessageName = "UploadImage", Description = "This Method Upload Imege")]
        [System.Xml.Serialization.XmlInclude(typeof(ImageResult))]
        public ImageResult UploadImage(String image, String TempToolID)
        {
            ImageResult Resault = new ImageResult();
            Resault.PicPath = "error";
            Image convertedImage = Base64ToImage(image);
            if (convertedImage != null)
            {
                Resault.PicPath = TempToolID + "H" + StringGeneration.GetString() + ".jpg";
                try
                {
                    convertedImage.Save(Server.MapPath("Images/" + Resault.PicPath), System.Drawing.Imaging.ImageFormat.Jpeg);
                    try
                    {
                        InsertIntoPictures(TempToolID, Resault.PicPath);


                        Resault.IsAdded = 1;

                    }
                    catch (Exception e)
                    {
                        Resault.PicPath = e.Message;
                    }
                }
                catch (Exception e)
                {
                    Resault.PicPath = e.Message;

                }
            }
            return Resault;

        }

        private Image Base64ToImage(String Base64String)
        {
            try
            {
                byte[] ImageBytes = Convert.FromBase64String(Base64String);
                using (var ms = new MemoryStream(ImageBytes, 0, ImageBytes.Length))
                {
                    ms.Write(ImageBytes, 0, ImageBytes.Length);
                    Image image = Image.FromStream(ms, true);
                    return image;
                }

            }
            catch (Exception e)
            {
                return null;

            }

        }




        public void InsertIntoPictures(String TempToolID,string picpath)
        {
            
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Pictures(ToolID,PicPath) VALUES (@ToolID,@PicPath)");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolID", TempToolID);
                    cmd.Parameters.AddWithValue("@PicPath", picpath);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }            

                 }





        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void AddTools(String UserID, String ToolName, String ToolDes, String ToolPrice, String ToolTypeID,
            String TempToolID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 1;
            String Message = "";
            String DateAdded = DateTime.Now.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Tools(ToolName,ToolDes,ToolTypeID,UserID,ToolPrice,DateAdd)VALUES (@ToolName,@ToolDes,@ToolTypeID,@UserID,@ToolPrice,@DateAdd)");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@ToolName", ToolName);
                    cmd.Parameters.AddWithValue("@ToolDes", ToolDes);
                    cmd.Parameters.AddWithValue("@ToolPrice", ToolPrice);
                    cmd.Parameters.AddWithValue("@ToolTypeID", ToolTypeID);
                    cmd.Parameters.AddWithValue("@DateAdd", DateAdded);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                //*******************************
                Tools Mytool = new Tools();
                String ToolID = Mytool.GetLastToolAdded(UserID, DateAdded);
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("update Pictures set ToolID=@ToolID where ToolID=@TempToolID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);
                    cmd.Parameters.AddWithValue("@TempToolID", TempToolID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                Message = "Is Added";


            }
            catch (Exception ex)
            {
                IsAdded = 0;
                Message = ex.Message;

            }


            var jsonData = new
            {
                IsAdded = IsAdded,
                Message = Message
            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));
        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetToolType()
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ToolType[] ToolData = null;
            try
            {
               // SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter("SELECT * FROM ToolType", connection);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);
                    ToolData = new ToolType[dataTable.Rows.Count];
                    int Count = 0;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ToolData[Count] = new ToolType();
                        ToolData[Count].ToolTypeID = Convert.ToInt32(dataTable.Rows[i]["ToolTypeID"]);
                        ToolData[Count].ToolTypeName = Convert.ToString(dataTable.Rows[i]["ToolTypeName"]);
                        Count++;

                    }
                    dataTable.Clear();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {

            }
            var jsonData = new
            {
                ToolData = ToolData
            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }





        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetToolListing(String UserID, String Distance, String From, String to, String ToolTypeID, String ToolID, String q)
        {
            if (q.Equals("@")) q = "%";
            else
                q = "%" + q + "%";
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ToolListing[] ToolData = null;
            int HasTool = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "ToolListing";
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@Distance", Distance);
                    cmd.Parameters.AddWithValue("@From", From);
                    cmd.Parameters.AddWithValue("@to", to);
                    cmd.Parameters.AddWithValue("@ToolTypeID", ToolTypeID);
                    cmd.Parameters.AddWithValue("@q", q);
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);
                    connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);
                    ToolData = new ToolListing[dataTable.Rows.Count];

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ToolData[i] = new ToolListing();
                        ToolData[i].ToolID = Convert.ToString(dataTable.Rows[i]["ToolID"]);
                        ToolData[i].ToolName = Convert.ToString(dataTable.Rows[i]["ToolName"]);
                        ToolData[i].ToolDes = Convert.ToString(dataTable.Rows[i]["ToolDes"]);
                        ToolData[i].ToolPrice = Convert.ToString(dataTable.Rows[i]["ToolPrice"]);
                        ToolData[i].DateAdd = Convert.ToString(dataTable.Rows[i]["DateAdd"]);
                        ToolData[i].PictureLink = Convert.ToString(dataTable.Rows[i]["PictureLink"]);

                    }
                    if (dataTable.Rows.Count > 0)
                    {
                        HasTool = 1;
                        dataTable.Clear();
                        connection.Close();

                    }

                }
            }
            catch (Exception ex)
            {
            }
            var jsonData = new
            {
                ToolData = ToolData,
                HasTool = HasTool
      
            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));
        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetToolDescription(String ToolID)
        {


            JavaScriptSerializer ser = new JavaScriptSerializer();
            ToolListingDescription ToolData = new ToolListingDescription();
            int HasTool = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM Tools where ToolID=@ToolID";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);
                    ToolData.ToolID = Convert.ToString(dataTable.Rows[0]["ToolID"]);
                    ToolData.ToolName = Convert.ToString(dataTable.Rows[0]["ToolName"]);
                    ToolData.ToolDes = Convert.ToString(dataTable.Rows[0]["ToolDes"]);
                    ToolData.ToolPrice = Convert.ToString(dataTable.Rows[0]["ToolPrice"]);
                    ToolData.DateAdd = Convert.ToString(dataTable.Rows[0]["DateAdd"]);
                    String UserID = Convert.ToString(dataTable.Rows[0]["UserID"]);
                    // get tool image
                    SqlCommand cmd1 = new SqlCommand();
                    cmd1.CommandText = "SELECT * FROM Pictures where ToolID=@ToolID";
                    cmd1.Parameters.AddWithValue("@ToolID", ToolID);
                    cmd1.Connection = connection;
                    adpt = new SqlDataAdapter(cmd1);
                    dataTable = new DataTable();
                    adpt.Fill(dataTable);
                    ToolData.PictureLinkar = new string[dataTable.Rows.Count];



                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ToolData.PictureLinkar[i] = Convert.ToString(dataTable.Rows[i]["PicPath"]);
                    }
                    //get user owner info
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.CommandText = "SELECT * FROM Users where UserID=@UserID";
                    cmd2.Parameters.AddWithValue("@UserID", UserID);
                    cmd2.Connection = connection;
                    adpt = new SqlDataAdapter(cmd2);
                    dataTable = new DataTable();
                    adpt.Fill(dataTable);
                    ToolData.PhoneNumber = Convert.ToString(dataTable.Rows[0]["PhoneNumber"]);
                    ToolData.Email = Convert.ToString(dataTable.Rows[0]["Email"]);
                    ToolData.Latitle = Convert.ToString(dataTable.Rows[0]["Latitle"]);
                    ToolData.Logtit = Convert.ToString(dataTable.Rows[0]["Logtit"]);
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
            }
            var jsonData = new
            {
                ToolData = ToolData

            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }






        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void RemoveTool(String ToolID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsRemoved = 1;
            String Message = "Iteme Removed from list";
            String DateAdded = DateTime.Now.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("delete from Tools where ToolID = @ToolID");
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);
                    cmd.Connection = connection;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
            }



            catch (Exception ex)
            {
                IsRemoved = 0;
                Message = ex.Message;

            }


            var jsonData = new
            {
                IsDeleted = IsRemoved,
                Message = Message
            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }




//********************************************************************************************

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetMyTool(String UserID)
        {

            JavaScriptSerializer ser = new JavaScriptSerializer();
            ToolListing[] ToolData = null;
            int HasTool = 0;
            Array[] tab = null;
            try
            {
                using (SqlConnection connextion = new SqlConnection(DBConnection.ConnectionString))
                {
                    
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Tools where UserID=@UserID");
                    cmd.CommandType = CommandType.Text;

                    cmd.Connection = connextion;
                    cmd.Parameters.AddWithValue("@UserID", UserID);

                    connextion.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable datatable = new DataTable();
                    adpt.Fill(datatable);
                    ToolData = new ToolListing[datatable.Rows.Count];

                    for (int i = 0; i < datatable.Rows.Count; i++)
                    {
                        ToolData[i] = new ToolListing();
                        ToolData[i].ToolID = Convert.ToString(datatable.Rows[i]["ToolID"]);
                        ToolData[i].ToolName = Convert.ToString(datatable.Rows[i]["ToolName"]);
                        ToolData[i].ToolDes = Convert.ToString(datatable.Rows[i]["ToolDes"]);
                        ToolData[i].ToolPrice = Convert.ToString(datatable.Rows[i]["ToolPrice"]);
                        ToolData[i].DateAdd = Convert.ToString(datatable.Rows[i]["DateAdd"]);

                        // get tool image
                           
                       
                        ToolData[i].PictureLink = PicLink(ToolData[i].ToolID);
                     
                       
                    }



                    if (datatable.Rows.Count > 0)
                    {
                        HasTool = 1;
                        datatable.Clear();
                        connextion.Close();

                    }
                    
                }
            }
            catch (Exception e)
            {
            }
            var jsonData = new
            {
                HasTool = HasTool,
                ToolData = ToolData
                
            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));



        }

        
        private String PicLink(String ToolID)
        {
            String gg = null;
            try
            {
                SqlDataReader reader ;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    
                    SqlCommand cmd = new SqlCommand("SELECT PicPath FROM Pictures where ToolID=@ToolID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ToolID", ToolID);

                    connection.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read() && gg == null)
                    {
                        gg = reader.GetString(0);
                    }
                   
                    reader.Close();

                    connection.Close();


                }
            }
            catch (Exception e)
            {
            }

            return gg;
        }


//*******************************************************************************************
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]

        public void SendEmail(string Email)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string username = string.Empty;
            string password = string.Empty;
            string meg = "";
            
            string Sender = "ecommerceapp.dz@gmail.com";
            using (SqlConnection connextion = new SqlConnection(DBConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Password , Username FROM Users WHERE Email = @Email"))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Connection = connextion;
                    connextion.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if(sdr.Read())
                        {
                            username = sdr["Username"].ToString();
                            password = sdr["Password"].ToString();

                        }
                    }
                    connextion.Close();
                }
            }
            if (!string.IsNullOrEmpty(password))
            {
                MailMessage mm = new MailMessage(Sender, Email);
                mm.Subject = "Password Recovrey";
                mm.Body = string.Format("Hello {0},<br /><br />Your Pssword is {1}.<br /><br />Thank You.", username, password);
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = Sender;
                NetworkCred.Password = "golonssowa";
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
                meg = "Passowrd has been sent to your email address.";
                
                
            }
            else {
                meg = "This email address does not match any user";
                
            }


            var jsonData = new
            {
                Message = meg

            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }

    }
}

    
