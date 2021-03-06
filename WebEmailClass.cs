using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Xml;


namespace MHP20ClassLib {

    public class WebEmailClass {
        /*
         * 
         * ومن فرعها في دبي، تقدم M: حالياً خدماتها لعملائها في المملكة العربية السعودية ومصر والعراق وأبوظبي والشارقة ورأس الخيمة. كما قدمت الشركة خدماتها خلال السنوات الأخيرة في الأراضي الفلسطينية وسوريا وعُمان. ولتعزيز ورفع مستوى عروض خدماتها الأساسية، تتمتع الشركة بعلاقات متميزة مع عدد من الشركاء المحليين في أنحاء منطقة الخليج، وبالتحديد في كل من البحرين وقطر والمملكة العربية السعودية ومصر والكويت. 

         * */
        private LoggingClass log = null;
        private SmtpClient client = null;
        private int timeout = 100000;
        private string logfolder = "";
        private bool loaded = true;

        public WebEmailClass(string LogFolder) {
            logfolder = LogFolder;
        }

        public WebEmailClass(string LogFolder, int m_timeout) {
            logfolder = LogFolder;
            timeout = m_timeout;
        }

        public ReturnClass Send(string toname, string toemail, string fromname, string fromemail, string subject, string body) {
            SimpleEmailVO VO = new SimpleEmailVO();
            ReturnClass outcome = new ReturnClass(true);
            if (StringFuncs.IsValidEmail(toemail) == true && StringFuncs.IsValidEmail(fromemail)) {

                VO.ToName = toname;
                VO.ToEmail = toemail;
                VO.FromName = fromname;
                VO.FromEmail = fromemail;
                VO.Subject = subject;
                VO.TextBody = body;
                VO.HtmlBody = body;
                outcome = Send(VO);
            } else {
                outcome.Success = false;
                outcome.Message = "One or both of the email addresses are invalid.";
            }

            return outcome;

        }

        public ReturnClass Send(SimpleEmailVO VO) {
            log = new LoggingClass(logfolder);
            ReturnClass outcome = new ReturnClass(true);

            if (StringFuncs.IsValidEmail(VO.ToEmail) == true && StringFuncs.IsValidEmail(VO.FromEmail)) {

                if (loaded == true) {
                    MailAddress toaddress = new MailAddress(VO.ToEmail, VO.ToName);
                    MailAddress fromaddress = new MailAddress(VO.FromEmail, VO.FromName);
                    MailMessage message = new MailMessage(fromaddress, toaddress);
                    message.Subject = VO.Subject;
                    if (VO.IsBodyHtml == true) {
                        message.Body = VO.HtmlBody;
                    } else {
                        message.Body = VO.TextBody;
                    }
                    message.IsBodyHtml = VO.IsBodyHtml;
                    message.Priority = MailPriority.Normal;
                    message.Headers.Add("Return-Path", VO.FromEmail);
                    try {
                        client = new SmtpClient();
                        if (timeout != 100000) {
                            client.Timeout = timeout;
                        }
                        //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                        //client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;

                    } catch (Exception ex) {
                        outcome.Success = false;
                        outcome.Message = "There was a problem creating the email component [" + ex.Message + "]";
                    }

                    if (outcome.Success == true) {
                        try {
                            client.Send(message);
                        } catch (SmtpException ex) {
                            log.Log("EmailClass - Error sending email to " + VO.ToName + " :" + ex.StatusCode + " [" + ex.Message + "][" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                            outcome.Success = false;
                            outcome.Message = "EmailClass - Email error " + ex.StatusCode + " [" + ex.Message + "]";
                        } catch (Exception ex) {
                            log.Log("EmailClass - Error sending email to " + VO.ToName + " :" + ex.Message + "[" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                            outcome.Success = false;
                            outcome.Message = "EmailClass - Email error  [" + ex.Message + "]";
                        }
                    }
                } else {
                    outcome.Success = false;
                    outcome.Message = "The mail component can not be created - most probably the mail configuration is incorrect";
                }
            } else {
                outcome.Success = false;
                outcome.Message = "One or both of the email addresses are invalid.";
            }
            return outcome;
        }

        public ReturnClass SendMessages(SimpleEmailVOs VOs) {
            MailAddress toaddress = null;
            MailAddress fromaddress = null;
            MailMessage message = null;
            log = new LoggingClass(logfolder);
            ReturnClass outcome = new ReturnClass(true);
            ReturnClass mailcome = new ReturnClass(true);
            if (loaded == true) {

                try {
                    client = new SmtpClient();
                } catch (Exception ex) {
                    outcome.Success = false;
                    outcome.Message = "There was a problem creating the email component [" + ex.Message + "]";
                }

                if (outcome.Success == true) {
                    List<SimpleEmailVO> Emails = VOs.Emails;
                    for (int i = 0; i < Emails.Count; i++) {
                        if (StringFuncs.IsValidEmail(Emails[i].ToEmail) == true && StringFuncs.IsValidEmail(Emails[i].FromEmail)) {
                            toaddress = new MailAddress(Emails[i].ToEmail, Emails[i].ToName);
                            fromaddress = new MailAddress(Emails[i].FromEmail, Emails[i].FromName);
                            message = new MailMessage(fromaddress, toaddress);
                            message.Subject = Emails[i].Subject;
                            message.Body = Emails[i].HtmlBody;
                            message.IsBodyHtml = Emails[i].IsBodyHtml;
                            try {
                                client.Send(message);
                            } catch (SmtpException ex) {
                                log.Log("EmailClass - Error sending email to " + Emails[i].ToName + " :" + ex.StatusCode + " [" + ex.Message + "][" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                                mailcome.Success = false;
                                mailcome.AddMessage("EmailClass - Email error " + ex.StatusCode + " [" + ex.Message + "][" + ex.InnerException + "]");
                            } catch (Exception ex) {
                                log.Log("EmailClass - Error sending email to " + Emails[i].ToName + " :" + ex.Message + "", "MHPNET2EmailLib.EmailClass");
                                mailcome.Success = false;
                                mailcome.AddMessage("EmailClass - Email error  [" + ex.Message + "][" + ex.InnerException + "]");
                            }
                        } else {
                            mailcome.Success = false;
                            mailcome.Message = "One or both of the email addresses are invalid.";
                        }
                    }
                }

                if (mailcome.Success == false) {
                    outcome.Success = false;
                    outcome.Message = mailcome.Message;
                }

            } else {
                outcome.Success = false;
                outcome.Message = "The mail component can not be created - most probably the mail configuration is incorrect";
            }
            return outcome;
        }

    }
    /// <summary>
    /// This
    /// </summary>
    public class WebEmailClass2 {
        private LoggingClass log = null;
        private SmtpClient client = null;

        private string logfolder = "";
        private bool UseSSL = false;
        private bool loaded = true;

        public WebEmailClass2(string LogFolder, bool useSSL) {
            logfolder = LogFolder;
            UseSSL = useSSL;
        }

        public ReturnClass Send(string toname, string toemail, string fromname, string fromemail, string subject, string body) {
            SimpleEmailVO VO = new SimpleEmailVO();
            ReturnClass outcome = new ReturnClass(true);
            if (StringFuncs.IsValidEmail(toemail) == true && StringFuncs.IsValidEmail(fromemail)) {

                VO.ToName = toname;
                VO.ToEmail = toemail;
                VO.FromName = fromname;
                VO.FromEmail = fromemail;
                VO.Subject = subject;
                VO.TextBody = body;
                VO.HtmlBody = body;
                outcome = Send(VO);
            } else {
                outcome.Success = false;
                outcome.Message = "One or both of the email addresses are invalid.";
            }

            return outcome;

        }

        public ReturnClass Send(SimpleEmailVO VO) {
            MailAddress toaddress = null;
            MailAddress fromaddress = null;
            MailAddress replyaddress = null;
            MailMessage message = null;
            log = new LoggingClass(logfolder);
            ReturnClass outcome = new ReturnClass(true);

            if (StringFuncs.IsValidEmail(VO.ToEmail) && StringFuncs.IsValidEmail(VO.FromEmail)) {

                if (loaded == true) {
                    try {
                        toaddress = new MailAddress(VO.ToEmail, VO.ToName);
                        fromaddress = new MailAddress(VO.FromEmail, VO.FromName);
                        message = new MailMessage(fromaddress, toaddress);
                    } catch (Exception ex) {
                        outcome.Success = false;
                        outcome.Message = "There was a problem creating the addresses or message [" + ex.Message + "]";
                    }
                    if (outcome.Success) {
                        try {
                            message.Subject = VO.Subject;

                            if (VO.IsBodyHtml == true) {
                                message.Body = VO.HtmlBody;
                            } else {
                                message.Body = VO.TextBody;
                            }
                            message.IsBodyHtml = VO.IsBodyHtml;
                            message.Priority = MailPriority.Normal;
                            message.Headers.Add("Return-Path", VO.FromEmail);
                        } catch (Exception ex) {
                            outcome.Success = false;
                            outcome.Message = "There was a problem setting the message parts(priority, return-path) [" + ex.Message + "]";
                        }
                    }
                    if (outcome.Success) {
                        if (VO.ReplyToEmail.Length > 0 && StringFuncs.IsValidEmail(VO.ReplyToEmail)) {
                            if (VO.ReplyToName.Length > 0) {
                                replyaddress = new MailAddress(VO.ReplyToEmail, VO.ReplyToName);
                            } else {
                                replyaddress = new MailAddress(VO.ReplyToEmail, VO.ReplyToEmail);
                            }
                            message.ReplyTo = replyaddress;
                        }
                    }

                    if (outcome.Success) {
                        try {
                            client = new SmtpClient();
                        } catch (Exception ex) {
                            outcome.Success = false;
                            outcome.Message = "There was a problem creating the email component [" + ex.Message + "]";
                        }
                    }
                    if (outcome.Success) {
                        if (UseSSL) {
                            client.EnableSsl = true;
                        }
                    }
                    if (outcome.Success) {
                        try {
                            client.Send(message);
                        } catch (SmtpException ex) {
                            log.Log("EmailClass - Error sending email to " + VO.ToName + " :" + ex.StatusCode + " [" + ex.Message + "][" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                            outcome.Success = false;
                            outcome.Message = "EmailClass - Email error " + ex.StatusCode + " [" + ex.Message + "]";
                        } catch (Exception ex) {
                            log.Log("EmailClass - Error sending email to " + VO.ToName + " :" + ex.Message + "[" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                            outcome.Success = false;
                            outcome.Message = "EmailClass - Email error  [" + ex.Message + "]";
                        }
                    }
                } else {
                    outcome.Success = false;
                    outcome.Message = "The mail component can not be created - most probably the mail configuration is incorrect";
                }
            } else {
                outcome.Success = false;
                outcome.Message = "One or both of the email addresses are invalid.";
            }
            return outcome;
        }

        public ReturnClass SendMessages(SimpleEmailVOs VOs) {
            MailAddress toaddress = null;
            MailAddress fromaddress = null;
            MailMessage message = null;
            log = new LoggingClass(logfolder);
            ReturnClass outcome = new ReturnClass(true);
            ReturnClass mailcome = new ReturnClass(true);
            if (loaded == true) {

                try {
                    client = new SmtpClient();
                } catch (Exception ex) {
                    outcome.Success = false;
                    outcome.Message = "There was a problem creating the email component [" + ex.Message + "]";
                }

                if (UseSSL) {
                    client.EnableSsl = true;
                }

                if (outcome.Success == true) {
                    List<SimpleEmailVO> Emails = VOs.Emails;
                    for (int i = 0; i < Emails.Count; i++) {
                        if (StringFuncs.IsValidEmail(Emails[i].ToEmail) == true && StringFuncs.IsValidEmail(Emails[i].FromEmail)) {
                            toaddress = new MailAddress(Emails[i].ToEmail, Emails[i].ToName);
                            fromaddress = new MailAddress(Emails[i].FromEmail, Emails[i].FromName);
                            message = new MailMessage(fromaddress, toaddress);
                            message.Subject = Emails[i].Subject;
                            message.Body = Emails[i].HtmlBody;
                            message.IsBodyHtml = Emails[i].IsBodyHtml;
                            try {
                                client.Send(message);
                            } catch (SmtpException ex) {
                                log.Log("EmailClass - Error sending email to " + Emails[i].ToName + " :" + ex.StatusCode + " [" + ex.Message + "][" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                                mailcome.Success = false;
                                mailcome.AddMessage("EmailClass - Email error " + ex.StatusCode + " [" + ex.Message + "][" + ex.InnerException + "]");
                            } catch (Exception ex) {
                                log.Log("EmailClass - Error sending email to " + Emails[i].ToName + " :" + ex.Message + "", "MHPNET2EmailLib.EmailClass");
                                mailcome.Success = false;
                                mailcome.AddMessage("EmailClass - Email error  [" + ex.Message + "][" + ex.InnerException + "]");
                            }
                        } else {
                            mailcome.Success = false;
                            mailcome.Message = "One or both of the email addresses are invalid.";
                        }
                    }
                }

                if (mailcome.Success == false) {
                    outcome.Success = false;
                    outcome.Message = mailcome.Message;
                }

            } else {
                outcome.Success = false;
                outcome.Message = "The mail component can not be created - most probably the mail configuration is incorrect";
            }
            return outcome;
        }

    }
/// <summary>
    /// This new class (Jan 2013) uses the AlternateView.CreateAlternateViewFromString method to add the html and text body versions, rather than the older syntax. It also includes a delivery email option
/// </summary>
    public class WebEmailClass3 {

        private string logfolder = "";
        private bool UseSSL = false;

        public WebEmailClass3(string LogFolder, bool useSSL) {
            logfolder = LogFolder;
            UseSSL = useSSL;
        }

        public ReturnClass Send(SimpleEmailVO VO) {
            return Send(VO, "");
        }

        public ReturnClass Send(SimpleEmailVO VO, string deliveryemail) {
            LoggingClass log = new LoggingClass(logfolder);
            ReturnClass outcome = new ReturnClass(true);
            AlternateView av1 = null;
            AlternateView av2 = null;
            SmtpClient mailSender = new SmtpClient();

            if (VO.FromName.Length == 0) {
                VO.FromName = VO.FromEmail;
            }
            if (VO.ToName.Length == 0) {
                VO.ToName = VO.ToEmail;
            }

            if (StringFuncs.IsValidEmail(VO.FromEmail) && StringFuncs.IsValidEmail(VO.ToEmail)) {

                MailMessage mailMessage = new MailMessage(new MailAddress(VO.FromEmail, VO.FromName), new MailAddress(VO.ToEmail, VO.ToName));

                if (StringFuncs.IsValidEmail(deliveryemail)) {
                    mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.Delay;
                    mailMessage.Headers.Add("Disposition-Notification-To", deliveryemail);

                    //mailMessage.Headers.Add("Errors-To", Config.EmailerDeliveryEmail);
                    //mailMessage.Headers.Add("Delivered-To", Config.EmailerDeliveryEmail);
                    //mailMessage.Headers.Add("Return-Receipt-Requested", Config.EmailerDeliveryEmail);
                    //mailMessage.Headers.Add("Return-Receipt-To", Config.EmailerDeliveryEmail);
                    //mailMessage.Headers.Add("Read-Receipt-Requested", Config.EmailerDeliveryEmail);
                    //mailMessage.Headers.Add("Read-Receipt-To", Config.EmailerDeliveryEmail);
                    //mailMessage.Headers.Add("X-Confirm-reading-to", Config.EmailerDeliveryEmail);
                }

                try {
                    if (VO.HtmlBody.Length > 0) {
                        if (VO.TextBody.Length > 0) {
                            av2 = AlternateView.CreateAlternateViewFromString(VO.TextBody, null, MediaTypeNames.Text.Plain);
                            mailMessage.AlternateViews.Add(av2);
                            av1 = AlternateView.CreateAlternateViewFromString(VO.HtmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
                            mailMessage.AlternateViews.Add(av1);
                        } else {
                            av1 = AlternateView.CreateAlternateViewFromString(VO.HtmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
                            mailMessage.AlternateViews.Add(av1);
                        }
                    } else {
                        av2 = AlternateView.CreateAlternateViewFromString(VO.TextBody, null, MediaTypeNames.Text.Plain);
                        mailMessage.AlternateViews.Add(av2);
                    }
                } catch (Exception ex) {
                    outcome.SetFailureMessage("Error setting email body:" + ex.ToString());
                }

                if (outcome.Success) {
                    try {
                        mailMessage.Subject = VO.Subject;
                        if (VO.HtmlBody.Length > 0) {
                            mailMessage.IsBodyHtml = true;
                        } else {
                            mailMessage.IsBodyHtml = false;
                        }
                        mailMessage.Priority = MailPriority.Normal;

                        if (StringFuncs.IsValidEmail(VO.ReplyToEmail)) {
                            mailMessage.Headers.Add("Return-Path", VO.ReplyToEmail);
                            mailMessage.ReplyTo = new MailAddress(VO.ReplyToEmail, VO.ReplyToEmail);
                        } else {
                            mailMessage.Headers.Add("Return-Path", VO.FromEmail);
                            mailMessage.ReplyTo = new MailAddress(VO.FromEmail, VO.FromName);
                        }
                        
                        mailSender.EnableSsl = UseSSL;

                    } catch (Exception ex) {
                        outcome.SetFailureMessage("Error setting message settings:" + ex.ToString());
                    }

                    if (outcome.Success) {
                        try {
                            mailSender.Send(mailMessage);
                        } catch (SmtpException ex) {
                            log.Log("EmailClass - Error sending email to " + VO.ToName + " :" + ex.StatusCode + " [" + ex.Message + "][" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                            outcome.SetFailureMessage("EmailClass - Email error " + ex.StatusCode + " [" + ex.Message + "]");
                        } catch (Exception ex) {
                            log.Log("EmailClass - Error sending email to " + VO.ToName + " :" + ex.Message + "[" + ex.InnerException + "]", "MHPNET2EmailLib.EmailClass");
                            outcome.SetFailureMessage("EmailClass - Email error  [" + ex.Message + "]");
                        }
                    }
                }
            } else {
                outcome.SetFailureMessage("Either the target (" + VO.ToEmail + ") or sender email (" + VO.FromEmail + ")is invalid");
            }

            return outcome;
        }


    }

    public class SimpleEmailVOs {
        public List<SimpleEmailVO> Emails = new List<SimpleEmailVO>();

        public SimpleEmailVOs() { 
        
        
        }

        public void Add(SimpleEmailVO vo) {
            Emails.Add(vo);
        }

        public void AddEmail(string toname, string toemail, string fromname, string fromemail, string subject, string body) {
            SimpleEmailVO VO = new SimpleEmailVO();
            VO.ToName = toname;
            VO.ToEmail = toemail;
            VO.FromName = fromname;
            VO.FromEmail = fromemail;
            VO.Subject = subject;
            VO.TextBody = body;
            VO.HtmlBody = body;
            Emails.Add(VO);
        }

        public void Remove(string id) {
            for (int i = 0; i < Emails.Count; i++) {
                if (Emails[i].Id == id) {
                    Emails.Remove(Emails[i]);
                }
            }
        }
    }

    public class SimpleEmailVO {
        public string Id = "";
        public string FromName = "";
        public string FromEmail = "";
        public string ToName = "";
        public string ToEmail = "";
        public string ReplyToName = "";
        public string ReplyToEmail = "";
        public string Subject = "";
        public string TextBody = "";
        public string HtmlBody = "";
        public bool IsBodyHtml = false;
        public SimpleEmailVO() { 
        
        }
    }

    public class EmailHelperClass {
        public string ApplicationName = "";
        public string Url = "";
        public string LogFolder = "";
        public Boolean SmtpSSL = false;
        public string FromName = "";
        public string FromEmail = "";
        public string ToName = "";
        public string ToEmail = "";
        public EmailHelperClass(string _LogFolder, bool _SmtpSSL, string _FromName, string _FromEmail, string _ToName, string _ToEmail) {
            LogFolder = _LogFolder;
            SmtpSSL = _SmtpSSL;
            FromName = _FromName;
            FromEmail = _FromName;
            ToName = _ToName;
            ToEmail = _ToEmail;
        }
    }

}
