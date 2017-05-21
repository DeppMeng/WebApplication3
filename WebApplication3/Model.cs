using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;

namespace WebApplication3.Controllers
{
    public class Model
    {
        public static void login()
        {
            try
            {
                string conn = "Server=202.38.88.99,1434;User ID=student;Password=student";
                PublicInfo.currentconnection = new SqlConnection(conn);
                PublicInfo.currentconnection.Open();
                PublicInfo.conn_result = "succeed";
            }
            catch (Exception ex)
            {
                PublicInfo.conn_result = ex.ToString();
            }
        }

        public static List<Item> ConvertToList(DataTable dt)
        {
            string tempName = string.Empty;
            List<Item> ts = new List<Item>();

            foreach (DataRow dr in dt.Rows)
            {
                Item t = new Item();
                t.Book_id = dr["Book_id"] as string;
                t.Book_name = dr["Book_name"] as string;
                t.Book_author = dr["Book_author"] as string;
                t.Book_publisher = dr["Book_publisher"] as string;
                t.Book_saletime = (dr["Book_saletime"] as int?) ?? 0;
                t.Book_status = (dr["Book_status"] as int?) ?? 0;

                ts.Add(t);
            }
            return ts;
        }

        public static int getNewSeed()
        {
            byte[] rndBytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return BitConverter.ToInt32(rndBytes, 0);
        }

        public static string GetRandomString(int len)
        {
            string s = "123456789abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";
            string reValue = string.Empty;
            Random rnd = new Random(getNewSeed());
            while (reValue.Length < len)
            {
                string s1 = s[rnd.Next(0, s.Length)].ToString();
                if (reValue.IndexOf(s1) == -1) reValue += s1;
            }
            return reValue;
        }

        public static void time_update()
        {
            DateTime current_time = new DateTime();
            current_time = DateTime.Now;
            PublicInfo.day = current_time.Day;
            PublicInfo.hour = current_time.Hour;
            PublicInfo.minute = current_time.Minute;
        }

        public static bool time_check()
        {
            DateTime current_time = new DateTime();
            current_time = DateTime.Now;
            if (PublicInfo.day != current_time.Day || PublicInfo.hour != current_time.Hour || Math.Abs(PublicInfo.minute - current_time.Minute) >= 15)
                return false;
            else
                return true;
        }

        public static void WriteLog(ActionLog actionlog)
        {
            string folder = "G:\\Study\\Database";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(string.Format("{0}\\{1}.txt", folder, DateTime.Now.ToString("yyyyMMdd")), "测试信息向文件中覆盖写入信息", Encoding.UTF8);

            //在将文本写入文件前，处理文本行
            //StreamWriter一个参数默认覆盖
            //StreamWriter第二个参数为false覆盖现有文件，为true则把文本追加到文件末尾
            using (StreamWriter file = new StreamWriter(string.Format("{0}\\{1}.txt", folder, DateTime.Now.ToString("dd")), true))
            {
                file.WriteLine(string.Format("User:{0} Action:{1} Book:{2} Time:{3}", actionlog.user_id, actionlog.action_type, actionlog.Book_id, actionlog.time));//直接追加文件末尾，不换行
                file.WriteLine("---------------------------------");
                file.WriteLine();// 直接追加文件末尾，换行
                file.Close();
            }
        }
    }
}