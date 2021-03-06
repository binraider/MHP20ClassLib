using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web;


namespace MHP20ClassLib {
    /*
 * 
 * ومن فرعها في دبي، تقدم M: حالياً خدماتها لعملائها في المملكة العربية السعودية ومصر والعراق وأبوظبي والشارقة ورأس الخيمة. كما قدمت الشركة خدماتها خلال السنوات الأخيرة في الأراضي الفلسطينية وسوريا وعُمان. ولتعزيز ورفع مستوى عروض خدماتها الأساسية، تتمتع الشركة بعلاقات متميزة مع عدد من الشركاء المحليين في أنحاء منطقة الخليج، وبالتحديد في كل من البحرين وقطر والمملكة العربية السعودية ومصر والكويت. 

 * */
    public class LoggingClass {

        private string loggingfilepath = "";

        public LoggingClass(string logfolder) {
            loggingfilepath = logfolder;
        }

        public void Log(string message, string fn) {
            try {
                Stream s = new FileStream(loggingfilepath + @"\" + fn + ".txt", FileMode.Append);
                TextWriter tw = new StreamWriter(s);
                tw.WriteLine(message);
                tw.Flush();
                s.Close();
            } catch (Exception ex) { 
            
            }
        }


    }
}
