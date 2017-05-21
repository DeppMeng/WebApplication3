using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Text;
using System.Reflection;

namespace WebApplication3.Controllers
{

    #region public class definitions

    public class Item
    {
        public string Book_id
        {
            get { return book_id; }
            set { book_id = value; }
        }
        public string Book_name
        {
            get { return book_name; }
            set { book_name = value; }
        }
        public int Book_left
        {
            get { return book_left; }
            set { book_left = value; }
        }
        public string Book_author
        {
            get { return book_author; }
            set { book_author = value; }
        }
        public string Book_publisher
        {
            get { return book_publisher; }
            set { book_publisher = value; }
        }
        public int Book_saletime
        {
            get { return book_saletime; }
            set { book_saletime = value; }
        }

        public int Book_status
        {
            get { return book_status; }
            set { book_status = value; }
        }

        private string book_id;
        private string book_name;
        private int book_left;
        private string book_author;
        private string book_publisher;
        private int book_saletime;
        private int book_status;
    }

    public static class PublicInfo
    {
        public static SqlConnection currentconnection = null;
        public static SqlDataAdapter sda = null;
        public static string temp_command = null;
        public static string user_id = null;
        public static string conn_result = null;
        public static string key = null;
        public static int day;
        public static int hour;
        public static int minute;
    }

    public class ReceiveContent
    {
        public int func_select = 0;
        public LoginContent login = new LoginContent();
        public SearchContent search = new SearchContent();
        public UpdateContent update = new UpdateContent();
        public LogoutContent logout = new LogoutContent();
        public RegisterContent register = new RegisterContent();
        public PersonalInfo personalinfo = new PersonalInfo();
        public NewBookContent newbook = new NewBookContent();
        public BookEliminationContent bookelimination = new BookEliminationContent();
        public string varification = null;
    }

    public class BookEliminationContent
    {
        public string book_id;
    }

    public class SendContent
    {
        public int func_select = 0;
        public string content = null;
        public List<Item> data = null;
        public string errormessage = null;
        public string key = null;
    }

    public class LoginContent
    {
        public int login_type;
        public string user_name = null;
        public string password = null;
    }

    public class SearchContent
    {
        public int match_type = 0;
        public string search_condition = null;
        public string keyword = null;
    }

    public class UpdateContent
    {
        public int update_type = 0;
        public string user = null;
        public string book_id = null;
    }

    public class LogoutContent
    {
        public string user = null;
    }

    public class RegisterContent
    {
        public string user_name = null;
        public string password = null;
        public string email = null;
    }

    public class PersonalInfo
    {
        public string username;
        public string email;
    }

    public class ActionLog
    {
        public string user_id;
        public string Book_id;
        public int action_type;
        public string time;
    }

    public class NewBookContent
    {
        public string name;
        public string author;
        public int price;
        public string publisher;
        public int publish_year;
    }

    #endregion

    public class ValuesController : ApiController
    {
        // POST api/values
        public SendContent Post([FromBody]string value)
        {
            ActionLog actionlog = new ActionLog();
            var receivecontent = JsonConvert.DeserializeObject<ReceiveContent>(value);
            SendContent sendcontent = new SendContent();
            DateTime current_time = new DateTime();
            Model.login();
            if (PublicInfo.conn_result != "succeed")
            {
                string sendmsg = System.String.Format("Login error: {0}", PublicInfo.conn_result);
                sendcontent.func_select = 1;
                sendcontent.content = sendmsg;
                return sendcontent;
            }
            if ( receivecontent.func_select == 0 || receivecontent.func_select == 1 || receivecontent.func_select == 3 || receivecontent.func_select == 4 )
            {
                if (receivecontent.func_select == 0)
                {
                    bool tag_temp;
                    string temp = null;
                    if (receivecontent.login.login_type == 0)
                    {
                        try
                        {

                            string temp_command = "Select password from PB14000314_user_1 Where username=@username";
                            SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                            cmd.Parameters.AddWithValue("@username", receivecontent.login.user_name);
                            cmd.CommandText = temp_command;

                            SqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            temp = reader["password"].ToString();
                            reader.Dispose();
                        }
                        catch
                        {
                            string sendmsg = "Login Error! User name does not exist.";
                            sendcontent.func_select = 1;
                            sendcontent.content = sendmsg;
                            return sendcontent;
                        }
                    }
                    else
                    {
                        try
                        {

                            string temp_command = "Select password from PB14000314_administrator Where admin_name=@username";
                            SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                            cmd.Parameters.AddWithValue("@username", receivecontent.login.user_name);
                            cmd.CommandText = temp_command;

                            SqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            temp = reader["password"].ToString();
                            reader.Dispose();
                        }
                        catch
                        {
                            string sendmsg = "Login Error! Administrator name does not exist.";
                            sendcontent.func_select = 1;
                            sendcontent.content = sendmsg;
                            return sendcontent;
                        }
                    }
                    tag_temp = System.String.Equals(receivecontent.login.password.GetHashCode().ToString(), temp);

                    string aaa = "Login succeed.";
                    string password_hash = receivecontent.login.password.GetHashCode().ToString();
                    if (!tag_temp)
                    {
                        string sendmsg = "Login error! Password incorrect.";
                        sendcontent.func_select = 1;
                        sendcontent.content = sendmsg;
                        return sendcontent;
                    }
                    else
                    {

                        SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                        string temp_command = null;
                        if (receivecontent.login.login_type == 0)
                            temp_command = "Select user_id from PB14000314_user_1 where username=@username";
                        else
                            temp_command = "Select admin_id from PB14000314_administrator where admin_name=@username";
                        cmd.Parameters.AddWithValue("@username", receivecontent.login.user_name);
                        cmd.CommandText = temp_command;
                        PublicInfo.sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        PublicInfo.sda.Fill(dt);
                        DataRow dr = dt.Rows[0];
                        if (receivecontent.login.login_type == 0)
                            PublicInfo.user_id = dr["user_id"] as string;
                        else
                            PublicInfo.user_id = dr["admin_id"] as string;

                        PublicInfo.key = Model.GetRandomString(16);
                        sendcontent.key = PublicInfo.key;
                        Model.time_update();
                        string sendmsg = aaa;
                        sendcontent.func_select = 1;
                        sendcontent.content = sendmsg;
                        return sendcontent;
                    }
                }
                if (receivecontent.func_select == 1)
                {
                    try
                    {
                        if (receivecontent.search.match_type == 0)
                        {
                            SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                            string temp_command = System.String.Format("Select Book_id, Book_name, Book_author, Book_publisher, Book_saletime, Book_status from PB14000314_bookinfo_1 Where {0}='{1}'", receivecontent.search.search_condition, receivecontent.search.keyword);
                            cmd.CommandText = temp_command;

                            PublicInfo.sda = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            PublicInfo.sda.Fill(dt);
                            sendcontent.data = Model.ConvertToList(dt);
                        }
                        else
                        {
                            SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                            string temp_command = System.String.Format("Select Book_id, Book_name, Book_author, Book_publisher, Book_saletime, Book_status from PB14000314_bookinfo_1 Where {0} Like '%{1}%'", receivecontent.search.search_condition, receivecontent.search.keyword);
                            cmd.CommandText = temp_command;

                            PublicInfo.sda = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            PublicInfo.sda.Fill(dt);
                            sendcontent.data = Model.ConvertToList(dt);
                        }
                        sendcontent.func_select = 2;
                    }
                    catch (Exception ex)
                    {
                        sendcontent.func_select = 0;
                        sendcontent.errormessage = ex.ToString();
                    }
                }
                if (receivecontent.func_select == 3)
                {
                    sendcontent.func_select = 1;
                    sendcontent.content = "Logout succeed.";
                    PublicInfo.key = null;
                    PublicInfo.user_id = null;
                }
                if (receivecontent.func_select == 4)
                {
                    try
                    {
                        string temp_command = "Select username from PB14000314_user_1 Where username=@username";
                        SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                        cmd.Parameters.AddWithValue("@username", receivecontent.register.user_name);
                        cmd.CommandText = temp_command;
                        PublicInfo.sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        dt = new DataTable();
                        PublicInfo.sda.Fill(dt);
                        DataRow dr = dt.Rows[0];
                        string temp_username = dr["username"] as string;
                        if (System.String.Equals(temp_username, receivecontent.register.user_name))
                        {
                            sendcontent.func_select = 1;
                            sendcontent.content = "The user name already existed, please change another name.";
                            return sendcontent;
                        }


                    }
                    catch
                    {
                        try
                        {
                            string temp_command = System.String.Format("Select count(user_id) as num from PB14000314_user_1");
                            SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                            cmd.CommandText = temp_command;
                            PublicInfo.sda = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            dt = new DataTable();
                            PublicInfo.sda.Fill(dt);
                            DataRow dr = dt.Rows[0];
                            int temp_usernum = (dr["num"] as int?) ?? 0;
                            temp_usernum += 1;
                            string user_id;
                            if (temp_usernum < 10)
                                user_id = System.String.Format("U000{0}", temp_usernum.ToString());
                            else if (temp_usernum < 100)
                                user_id = System.String.Format("U00{0}", temp_usernum.ToString());
                            else
                            {
                                sendcontent.func_select = 1;
                                sendcontent.content = "Sorry, database is full.";
                                return sendcontent;
                            }

                            temp_command = System.String.Format("Insert into PB14000314_user_1 values(@username, @email, @password, '{0}', 0)", user_id);
                            SqlCommand cmdtemp1 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                            cmdtemp1.Parameters.AddWithValue("@username", receivecontent.register.user_name);
                            cmdtemp1.Parameters.AddWithValue("@email", receivecontent.register.email);
                            cmdtemp1.Parameters.AddWithValue("@password", receivecontent.register.password.GetHashCode().ToString());
                            cmdtemp1.CommandType = CommandType.Text;
                            cmdtemp1.ExecuteNonQuery();
                            sendcontent.func_select = 1;
                            sendcontent.content = "Register succeed.";
                        }
                        catch (Exception ex)
                        {
                            sendcontent.func_select = 0;
                            sendcontent.errormessage = ex.ToString();
                        }
                    }
                }
            }
            else
            {
                if (!String.Equals(receivecontent.varification, PublicInfo.key) || PublicInfo.key == null || !Model.time_check())
                {
                    PublicInfo.key = null;
                    sendcontent.func_select = 9;
                    sendcontent.errormessage = "Please login first.";
                    return sendcontent;
                }
                else
                    Model.time_update();
                if (receivecontent.func_select == 2)
                {
                    try
                    {
                        if (receivecontent.update.update_type == 0)
                        {
                            SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                            string temp_command = System.String.Format("Select owned_num from PB14000314_user_1 where user_id='{0}'", PublicInfo.user_id);
                            cmd.CommandText = temp_command;
                            PublicInfo.sda = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            PublicInfo.sda.Fill(dt);
                            DataRow dr = dt.Rows[0];
                            int temp_num = (dr["owned_num"] as int?) ?? 0;

                            temp_command = "Select Book_status from PB14000314_bookinfo_1 where Book_id=@book_id";
                            cmd.CommandText = temp_command;
                            cmd.Parameters.AddWithValue("@book_id", receivecontent.update.book_id);
                            PublicInfo.sda = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            PublicInfo.sda.Fill(dt);
                            dr = dt.Rows[0];
                            current_time = DateTime.Now;
                            int temp_booknum = (dr["Book_status"] as int?) ?? 0;
                            if (temp_booknum == 0)
                            {
                                if (temp_num < 10)
                                {
                                    temp_command = System.String.Format("update PB14000314_user_1 set owned_num = owned_num + 1 where user_id='{0}'", PublicInfo.user_id);
                                    SqlCommand cmdtemp1 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                                    cmdtemp1.CommandType = CommandType.Text;
                                    cmdtemp1.ExecuteNonQuery();
                                    current_time = DateTime.Now;
                                    temp_command = System.String.Format("insert into PB14000314_rent values('{0}', @book_id, {1}, {2}, {3})", PublicInfo.user_id, current_time.Year, current_time.Month, current_time.Day);
                                    SqlCommand cmdtemp2 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                                    cmdtemp2.Parameters.AddWithValue("@book_id", receivecontent.update.book_id);
                                    cmdtemp2.CommandType = CommandType.Text;
                                    cmdtemp2.ExecuteNonQuery();
                                    temp_command = "update PB14000314_bookinfo_1 set Book_status = 1 where Book_id=@book_id";
                                    SqlCommand cmdtemp3 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                                    cmdtemp3.Parameters.AddWithValue("@book_id", receivecontent.update.book_id);
                                    cmdtemp3.CommandType = CommandType.Text;
                                    cmdtemp3.ExecuteNonQuery();
                                    sendcontent.func_select = 1;
                                    sendcontent.content = "Borrow completed.";
                                }
                                else
                                {
                                    sendcontent.func_select = 1;
                                    sendcontent.content = "You've already had 10 books, please return some first.";
                                }
                            }
                            else
                            {
                                sendcontent.func_select = 1;
                                sendcontent.content = "Sorry, this book is unavailable now.";
                            }
                        }
                        if (receivecontent.update.update_type == 1)
                        {
                            SqlCommand cmd1 = PublicInfo.currentconnection.CreateCommand();
                            string temp_command = System.String.Format("Select user_id from PB14000314_rent where user_id='{0}' and Book_id = @book_id", PublicInfo.user_id);
                            cmd1.CommandText = temp_command;
                            cmd1.Parameters.AddWithValue("@book_id", receivecontent.update.book_id);
                            PublicInfo.sda = new SqlDataAdapter(cmd1);
                            DataTable dt = new DataTable();
                            PublicInfo.sda.Fill(dt);
                            DataRow dr = dt.Rows[0];
                            string tempstring = dr["user_id"] as string;
                            if (tempstring == PublicInfo.user_id)
                            {
                                temp_command = System.String.Format("update PB14000314_user_1 set owned_num = owned_num - 1 where user_id='{0}'", PublicInfo.user_id);
                                SqlCommand cmdtemp1 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                                cmdtemp1.CommandType = CommandType.Text;
                                cmdtemp1.ExecuteNonQuery();
                                temp_command = System.String.Format("delete from PB14000314_rent where user_id = '{0}' and Book_id = @book_id", PublicInfo.user_id);
                                SqlCommand cmdtemp2 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                                cmdtemp2.Parameters.AddWithValue("@book_id", receivecontent.update.book_id);
                                cmdtemp2.CommandType = CommandType.Text;
                                cmdtemp2.ExecuteNonQuery();
                                temp_command = "update PB14000314_bookinfo_1 set Book_status = 0 where Book_id=@book_id";
                                SqlCommand cmdtemp3 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                                cmdtemp3.Parameters.AddWithValue("@book_id", receivecontent.update.book_id);
                                cmdtemp3.CommandType = CommandType.Text;
                                cmdtemp3.ExecuteNonQuery();
                                sendcontent.func_select = 1;
                                sendcontent.content = "Return completed.";
                            }
                            else
                            {
                                sendcontent.func_select = 1;
                                sendcontent.content = "Sorry but you do not have this book, please check the book ID.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sendcontent.func_select = 0;
                        sendcontent.errormessage = ex.ToString();
                    }
                }
                if (receivecontent.func_select == 5)
                {
                    try
                    {
                        SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                        string temp_command = "Select PB14000314_bookinfo_1.Book_id, Book_name, Book_author, Book_publisher, Book_saletime, Book_status from PB14000314_bookinfo_1, PB14000314_user_1, PB14000314_rent Where PB14000314_bookinfo_1.Book_id = PB14000314_rent.Book_id and PB14000314_user_1.user_id = PB14000314_rent.user_id and username = @username";
                        cmd.CommandText = temp_command;
                        cmd.Parameters.AddWithValue("@username", receivecontent.personalinfo.username);
                        PublicInfo.sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        PublicInfo.sda.Fill(dt);
                        sendcontent.data = Model.ConvertToList(dt);
                        SqlCommand cmd1 = PublicInfo.currentconnection.CreateCommand();
                        temp_command = System.String.Format("Select email from PB14000314_user_1 where user_id = '{0}'", PublicInfo.user_id);
                        cmd1.CommandText = temp_command;
                        PublicInfo.sda = new SqlDataAdapter(cmd1);
                        dt = new DataTable();
                        PublicInfo.sda.Fill(dt);
                        DataRow dr = dt.Rows[0];
                        string temp_email = dr["email"] as string;

                        sendcontent.content = temp_email;
                        sendcontent.func_select = 3;
                    }
                    catch (Exception ex)
                    {
                        sendcontent.func_select = 0;
                        sendcontent.errormessage = ex.ToString();
                    }
                }
                if (receivecontent.func_select == 6)
                {
                    try
                    {
                        string temp_command = System.String.Format("update PB14000314_user_1 set username = @username where user_id='{0}'", PublicInfo.user_id);
                        SqlCommand cmdtemp1 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                        cmdtemp1.Parameters.AddWithValue("@username", receivecontent.personalinfo.username);
                        cmdtemp1.CommandType = CommandType.Text;
                        cmdtemp1.ExecuteNonQuery();
                        sendcontent.func_select = 1;
                        sendcontent.content = "Username edit completed.";
                    }
                    catch (Exception ex)
                    {
                        sendcontent.func_select = 0;
                        sendcontent.errormessage = ex.ToString();
                    }
                }
                if (receivecontent.func_select == 7)
                {
                    try
                    {
                        string temp_command = System.String.Format("update PB14000314_user_1 set email = @email where user_id='{0}'", PublicInfo.user_id);
                        SqlCommand cmdtemp1 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                        cmdtemp1.Parameters.AddWithValue("@email", receivecontent.personalinfo.email);
                        cmdtemp1.CommandType = CommandType.Text;
                        cmdtemp1.ExecuteNonQuery();
                        sendcontent.func_select = 1;
                        sendcontent.content = "Email address edit completed.";
                    }
                    catch (Exception ex)
                    {
                        sendcontent.func_select = 0;
                        sendcontent.errormessage = ex.ToString();
                    }
                }
                if (receivecontent.func_select == 8)
                {
                    try
                    {
                        string temp_command = System.String.Format("Select count(Book_id) as num from PB14000314_bookinfo_1");
                        SqlCommand cmd = PublicInfo.currentconnection.CreateCommand();
                        cmd.CommandText = temp_command;
                        PublicInfo.sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        dt = new DataTable();
                        PublicInfo.sda.Fill(dt);
                        DataRow dr = dt.Rows[0];
                        int temp_usernum = (dr["num"] as int?) ?? 0 + 1;
                        temp_usernum += 1;
                        string user_id;
                        if (temp_usernum < 10)
                            user_id = System.String.Format("B000{0}", temp_usernum.ToString());
                        else if (temp_usernum < 100)
                            user_id = System.String.Format("B00{0}", temp_usernum.ToString());
                        else
                        {
                            sendcontent.func_select = 1;
                            sendcontent.content = "Sorry, database is full.";
                            return sendcontent;
                        }

                        temp_command = System.String.Format("Insert into PB14000314_bookinfo_1 values('{0}', @bookname, @price, @author, @publisher, @publish_year, 0)", user_id);
                        SqlCommand cmdtemp1 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                        cmdtemp1.Parameters.AddWithValue("@bookname", receivecontent.newbook.name);
                        cmdtemp1.Parameters.AddWithValue("@price", receivecontent.newbook.price);
                        cmdtemp1.Parameters.AddWithValue("@author", receivecontent.newbook.author);
                        cmdtemp1.Parameters.AddWithValue("@publisher", receivecontent.newbook.publisher);
                        cmdtemp1.Parameters.AddWithValue("@publish_year", receivecontent.newbook.publish_year);
                        cmdtemp1.CommandType = CommandType.Text;
                        cmdtemp1.ExecuteNonQuery();
                        sendcontent.func_select = 1;
                        sendcontent.content = "Book addition succeed.";
                    }
                    catch(Exception ex)
                    {
                        sendcontent.func_select = 0;
                        sendcontent.errormessage = ex.ToString();
                    }
                }
                if (receivecontent.func_select == 9)
                {
                    try
                    {
                        string temp_command = "delete from PB14000314_bookinfo_1 where Book_id = @book_id";
                        SqlCommand cmdtemp1 = new SqlCommand(temp_command, PublicInfo.currentconnection);
                        cmdtemp1.Parameters.AddWithValue("@book_id", receivecontent.bookelimination.book_id);
                        cmdtemp1.CommandType = CommandType.Text;
                        cmdtemp1.ExecuteNonQuery();
                        sendcontent.func_select = 1;
                        sendcontent.content = "Book elimination succeed.";
                    }
                    catch(Exception ex)
                    {
                        sendcontent.func_select = 0;
                        sendcontent.errormessage = ex.ToString();
                    }
                }
            }
            current_time = new DateTime();
            current_time = DateTime.Now;
            actionlog.action_type = (sendcontent.func_select == 0) ? receivecontent.func_select : 0;
            actionlog.Book_id = receivecontent.update.book_id;
            actionlog.user_id = PublicInfo.user_id;
            actionlog.time = current_time.ToString("u");
            Model.WriteLog(actionlog);
            return sendcontent;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
