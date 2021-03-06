using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MHP20ClassLib {
    /*
 * 
 * ومن فرعها في دبي، تقدم M: حالياً خدماتها لعملائها في المملكة العربية السعودية ومصر والعراق وأبوظبي والشارقة ورأس الخيمة. كما قدمت الشركة خدماتها خلال السنوات الأخيرة في الأراضي الفلسطينية وسوريا وعُمان. ولتعزيز ورفع مستوى عروض خدماتها الأساسية، تتمتع الشركة بعلاقات متميزة مع عدد من الشركاء المحليين في أنحاء منطقة الخليج، وبالتحديد في كل من البحرين وقطر والمملكة العربية السعودية ومصر والكويت. 

 * */
    public class FailoverClass {

        public FailoverClass() {
            
        }

        public bool TestConnectString(string Connstr1) {
            string ConnstrAdjusted = "";
            bool UsePrimary = true;
            SqlConnection conn = null;
            StringBuilder sb = new StringBuilder();
            string[] arr = null;
            
            if (Connstr1.IndexOf("Connection Timeout") > -1) {
                arr = Connstr1.Split(';');
                for (int i = 0; i < arr.Length; i++) {
                    if (arr[i].Trim().IndexOf("Connection Timeout") > -1) {

                    } else {
                        sb.Append(arr[i] + ";");
                    }
                }
                sb.Append("Connection Timeout=2;");
                ConnstrAdjusted = sb.ToString();
            } else {
                ConnstrAdjusted = Connstr1;
            }

            try {
                conn = new SqlConnection(ConnstrAdjusted);
                conn.Open();
            } catch (SqlException ex) {
                UsePrimary = false;
            } finally {
                conn.Close();
                conn.Dispose();
            }
            return UsePrimary;
        }

        public DataAccess GetDataAccess(string Connstr1, string Connstr2) {
            string ConnstrAdjusted = "";
            bool UsePrimary = true;
            SqlConnection conn = null;
            DataAccess db;
            StringBuilder sb = new StringBuilder();
            string[] arr = null;

            if (Connstr1.IndexOf("Connection Timeout") > -1) {
                arr = Connstr1.Split(';');
                for (int i = 0; i < arr.Length; i++) {
                    if (arr[i].Trim().IndexOf("Connection Timeout") > -1) {

                    } else {
                        sb.Append(arr[i] + ";");
                    }
                }
                sb.Append("Connection Timeout=2;");
                ConnstrAdjusted = sb.ToString();
            } else {
                ConnstrAdjusted = Connstr1;
            }

            try {
                conn = new SqlConnection(ConnstrAdjusted);
                conn.Open();
            } catch (SqlException ex) {
                UsePrimary = false;
            } finally {
                conn.Close();
                conn.Dispose();
            }

            if (UsePrimary == true) {
                db = new DataAccess(Connstr1);
            }else{
                db = new DataAccess(Connstr2);
            }
            return db;
        }


    }
}
