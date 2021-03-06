using System;
using System.IO;
using System.Messaging;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;

namespace MHP20ClassLib {

    /*
 * 
 * ومن فرعها في دبي، تقدم M: حالياً خدماتها لعملائها في المملكة العربية السعودية ومصر والعراق وأبوظبي والشارقة ورأس الخيمة. كما قدمت الشركة خدماتها خلال السنوات الأخيرة في الأراضي الفلسطينية وسوريا وعُمان. ولتعزيز ورفع مستوى عروض خدماتها الأساسية، تتمتع الشركة بعلاقات متميزة مع عدد من الشركاء المحليين في أنحاء منطقة الخليج، وبالتحديد في كل من البحرين وقطر والمملكة العربية السعودية ومصر والكويت. 

 * */

    [Serializable]
    public class DBScheduleVO {
        public int Databaseid = 0;
        public int Actionid = 0;
        public DateTime DateInitiated = DateTime.Now;
        public string Title = "";
        public string ActionDescription = "";
        public string MessageID = "";
        public bool RemoveException = false;
        public string DatabaseName = "";

        public DBScheduleVO() {

        }
    }

    [Serializable]
    public class DatabaseMessageObject {
        public int Rowid = 0;
        public string MessageID = "";
        public int ID = 0;
        public string Text = "";
        public bool Success = true;

        public DatabaseMessageObject() {

        }
        public DatabaseMessageObject(string _s) {
            Text = _s;
        }
        public DatabaseMessageObject(int _rowid, bool _success, string _s) {
            Rowid = _rowid;
            Success = _success;
            Text = _s;
        }
        public DatabaseMessageObject(bool _success) {
            Success = _success;
        }
    }

    public class ProcessDatabaseMessages {
        public ProcessDatabaseMessages() { }

        public List<DBScheduleVO> PeekDBScheduleVOs(string messagequeue, bool fulllogging, string logfolder) {
            List<DBScheduleVO> ReturnMessages = new List<DBScheduleVO>();
            bool cont = true;
            string errmsg = "";
            MessageQueue MyMessageQ = null; ;
            Message MyMessage = null;
            Message[] messages = null;
            DBScheduleVO obj = null;

            try {
                MyMessageQ = new MessageQueue(messagequeue);
                MyMessageQ.Formatter = new System.Messaging.BinaryMessageFormatter();
            } catch (MessageQueueException ex) {
                cont = false;
                errmsg = "There was an error accessing the message queue :[" + ex.Message + "]";
            }

            if (cont == true) {
                try {
                    messages = MyMessageQ.GetAllMessages();
                } catch (MessageQueueException ex) {
                    cont = false;
                    errmsg = "There was an error receiving messages from the queue :[" + ex.Message + "]";

                }
            }

            if (messages != null) {
                for (int i = 0; i < messages.Length; i++) {
                    try {
                        MyMessage = messages[i];
                        MyMessage.Formatter = new System.Messaging.BinaryMessageFormatter();
                        obj = (DBScheduleVO)MyMessage.Body;
                        obj.MessageID = MyMessage.Id;
                        ReturnMessages.Add(obj);
                    } catch (Exception ex) {
                        errmsg += " " + ex.Message + " ";
                    }
                }
            }

            if (errmsg.Length > 0) {
                Log("Error in PeekMessages[" + errmsg + "]", fulllogging, logfolder);
            }
            return ReturnMessages;
        }

        public List<DatabaseMessageObject> PeekMessages(string messagequeue, bool fulllogging, string logfolder) {
            List<DatabaseMessageObject> ReturnMessages = new List<DatabaseMessageObject>();
            bool cont = true;
            string errmsg = "";
            MessageQueue MyMessageQ = null; ;
            Message MyMessage = null;
            Message[] messages = null;
            TimeSpan span = new TimeSpan(1000);
            DatabaseMessageObject obj = null;

            try {
                MyMessageQ = new MessageQueue(messagequeue);
                MyMessageQ.Formatter = new System.Messaging.BinaryMessageFormatter();
            } catch (MessageQueueException ex) {
                cont = false;
                errmsg = "There was an error accessing the message queue :[" + ex.Message + "]";
            }

            if (cont == true) {
                try {
                    messages = MyMessageQ.GetAllMessages();
                } catch (MessageQueueException ex) {
                    cont = false;
                    errmsg = "There was an error receiving messages from the queue :[" + ex.Message + "]";

                }
            }

            if (messages != null) {
                for (int i = 0; i < messages.Length; i++) {
                    try {
                        MyMessage = messages[i];
                        MyMessage.Formatter = new System.Messaging.BinaryMessageFormatter();
                        obj = (DatabaseMessageObject)MyMessage.Body;
                        obj.MessageID = MyMessage.Id;
                        ReturnMessages.Add(obj);
                    } catch (Exception ex) {
                        errmsg += " " + ex.Message + " ";
                    }
                }
            }

            if (errmsg.Length > 0) {
                Log("Error in PeekMessages[" + errmsg + "]", fulllogging, logfolder);
            }
            return ReturnMessages;
        }

        public void DeleteMessage(string messagequeue, string[] messageids, string logfolder) {
            Message disposemsg = null;
            MessageQueue MyMessageQ = null;
            MyMessageQ = new MessageQueue(messagequeue);
            MyMessageQ.MessageReadPropertyFilter.SetAll();
            MyMessageQ.Formatter = new System.Messaging.BinaryMessageFormatter();
            for (int i = 0; i < messageids.Length; i++) {
                disposemsg = MyMessageQ.ReceiveById(messageids[i]);
            }
        }

        private void Log(string msg, bool fulllogging, string logfolder) {
            Stream s = null;
            TextWriter tw = null;
            if (fulllogging == true) {
                try {
                    s = new FileStream(logfolder + @"\DatabaseBackupApplicationMSMQ.txt", FileMode.Append);
                    tw = new StreamWriter(s);
                    tw.WriteLine(msg);
                    tw.Flush();
                    s.Close();
                } catch (Exception ex) {

                }
            }
        }

    }
}
