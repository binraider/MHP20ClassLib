using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Messaging;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;

namespace MHP20ClassLib {

    /*
 * 
 * ومن فرعها في دبي، تقدم M: حالياً خدماتها لعملائها في المملكة العربية السعودية ومصر والعراق وأبوظبي والشارقة ورأس الخيمة. كما قدمت الشركة خدماتها خلال السنوات الأخيرة في الأراضي الفلسطينية وسوريا وعُمان. ولتعزيز ورفع مستوى عروض خدماتها الأساسية، تتمتع الشركة بعلاقات متميزة مع عدد من الشركاء المحليين في أنحاء منطقة الخليج، وبالتحديد في كل من البحرين وقطر والمملكة العربية السعودية ومصر والكويت. 

 * */

    public class MSMQEmailerClient {

        public MSMQEmailerClient() { 
        
        }

        public ReturnClass SendMSMQMessage(EmailItemClass obj, string queuepath, string info) {

            ReturnClass outcome = new ReturnClass(true);
            MessageQueue MyMessageQ = null;
            Message MyMessage = null;

            try {
                MyMessageQ = new MessageQueue(queuepath);
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Techmessage = "Error connecting to message queue (" + queuepath + ") Error:[" + ex.Message + "]";
            }
            if (outcome.Success == true) {
                try {
                    MyMessage = new Message();
                    MyMessage.UseDeadLetterQueue = true;
                    MyMessage.Formatter = new System.Messaging.BinaryMessageFormatter();
                    MyMessage.Body = obj;
                } catch (Exception ex) {
                    outcome.Success = false;
                    outcome.Techmessage = "Error creating Message for queue. Error:[" + ex.Message + "]";
                }
            }
            if (outcome.Success == true) {
                try {
                    MyMessageQ.Send(MyMessage, "Message from MSMQ Email Service. Info:[" + info + "]");
                } catch (Exception ex) {
                    outcome.Success = false;
                    outcome.Techmessage = "Error sending message. Error:[" + ex.Message + "]";
                }
            }
            return outcome;
        }
    }

    [Serializable]
    public class EmailItemClass {
        public string EmailTo = "";
        public string EmailFrom = "";
        public string NameTo = "";
        public string NameFrom = "";
        public string Subject = "";
        public string Body = "";
        public bool HtmlEmail = true;
        public DateTime DateCreated;
        public DateTime DateSent;

        public EmailItemClass() {

        }
    }

}
