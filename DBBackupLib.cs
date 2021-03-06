using System;
using System.Configuration;
using System.Messaging;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Text;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MHP20ClassLib {
    /*
 * 
 * ومن فرعها في دبي، تقدم M: حالياً خدماتها لعملائها في المملكة العربية السعودية ومصر والعراق وأبوظبي والشارقة ورأس الخيمة. كما قدمت الشركة خدماتها خلال السنوات الأخيرة في الأراضي الفلسطينية وسوريا وعُمان. ولتعزيز ورفع مستوى عروض خدماتها الأساسية، تتمتع الشركة بعلاقات متميزة مع عدد من الشركاء المحليين في أنحاء منطقة الخليج، وبالتحديد في كل من البحرين وقطر والمملكة العربية السعودية ومصر والكويت. 

 * */
    public class DBBackupLib {

        private DBConfigClass config = null;

        public DBBackupLib(DBConfigClass _config) {
            config = _config;
        }

        public ReturnClass SimpleBackup() {
            ReturnClass outcome = BackupToLocalA();
            return outcome;
        }

        public ReturnClass SimpleRestore() {
            ReturnClass outcome = RestoreLocalServerFromA();
            return outcome;
        }


        public ReturnClass BackupToLocal() {
            string message = "";
            ReturnClass outcome = BackupToLocalB();
            return outcome;
        }

        public ReturnClass BackupToRemote() {
            string message = "";
            ReturnClass outcome = BackupToLocalB();
            if (outcome.Success == true) {
                outcome = CopyLocalToRemote();
                if (outcome.Success == false) {
                    message = outcome.Message + " " + outcome.Techmessage;
                    outcome.Message = "BackupToRemote Failed. Backup succeeded - File copy failed (part 2) >> " + message;
                }
            } else {
                message = outcome.Message + " " + outcome.Techmessage;
                outcome.Message = "BackupToRemote Failed. Backup section failed (part 1) >> " + message;
            }
            return outcome;
        }

        public ReturnClass BackupToRemoteAndRestore() {
            string message = "";
            ReturnClass outcome = BackupToLocalB();
            if (outcome.Success == true) {
                outcome = CopyLocalToRemote();
                if (outcome.Success == true) {
                    outcome = RestoreRemoteServer();
                    if (outcome.Success == false) {
                        message = outcome.Message + " " + outcome.Techmessage;
                        outcome.Message = "BackupToRemote Failed. Backup succeeded - File copy succeeded  - Restore Failed (part 3) >> " + message;
                    }
                }else{
                    message = outcome.Message + " " + outcome.Techmessage;
                    outcome.Message = "BackupToRemote Failed. Backup succeeded - File copy failed (part 2) >> " + message;
                }
            } else {
                message = outcome.Message + " " + outcome.Techmessage;
                outcome.Message = "BackupToRemote Failed. Backup section failed (part 1) >> " + message;
            }
            return outcome;
        }

        public ReturnClass RestoreFromRemoteServer() {
            string message = "";
            ReturnClass outcome = BackupRemote();
            if (outcome.Success == true) {
                outcome = CopyRemoteToLocal();
                if (outcome.Success == true) {
                    outcome = RestoreLocalServerFromB();
                    if (outcome.Success == false) {
                        message = outcome.Message + " " + outcome.Techmessage;
                        outcome.Message = "RestoreFromRemoteServer Failed. Backup Succeeded, File move succeeded, local restore failed (part 3) >> " + message;
                    }
                } else {
                    message = outcome.Message + " " + outcome.Techmessage;
                    outcome.Message = "RestoreFromRemoteServer Failed. Backup Succeeded, File move failed (part 2) >> " + message;
                }
            } else {
                message = outcome.Message + " " + outcome.Techmessage;
                outcome.Message = "RestoreFromRemoteServer Failed. Backup Failed (part 1) >> " + message;
            }

            return outcome;
        }

        public ReturnClass ActionById(int actionid) {
            ReturnClass outcome = new ReturnClass(true);
            switch (actionid) {
                case 1:
                    outcome = BackupToLocalA();
                    break;
                case 2:
                    outcome = RestoreLocalServerFromA();
                    break;
                case 3:
                    outcome = BackupToLocalB();
                    break;
                case 4:
                    outcome = BackupToRemote();
                    break;
                case 5:
                    outcome = BackupToRemoteAndRestore();
                    break;
                case 6:
                    outcome = RestoreFromRemoteServer();
                    break;

            }
            return outcome;
        }



        private ReturnClass BackupToLocalA() {
            ReturnClass outcome = new ReturnClass(true);
            SqlConnection conn = null;
            SqlCommand command = null;
            string Sql = "";
            int results = 0;
            try {
                Sql = "Backup Database " + config.DatabaseName + " to disk='" + config.BACKUPLOCAL1 + "' WITH INIT";
                conn = new SqlConnection(config.LocalConnectionString);
                command = new SqlCommand(Sql, conn);
                if (conn.State != ConnectionState.Open) conn.Open();
                try {
                    results = command.ExecuteNonQuery();
                } catch (Exception ex) {
                    outcome.Success = false;
                    outcome.Message = "Backup local database failed: database:" + config.DatabaseName;
                    outcome.Techmessage = "Backup local database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                }
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Backup local database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Backup local database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }

        private ReturnClass BackupToLocalB() {
            ReturnClass outcome = new ReturnClass(true);
            SqlConnection conn = null;
            SqlCommand command = null;
            string Sql = "";
            int results = 0;
            try{
                Sql = "Backup Database " + config.DatabaseName + " to disk='" + config.BACKUPLOCAL2 + "' WITH INIT";
                conn = new SqlConnection(config.LocalConnectionString);
                command = new SqlCommand(Sql, conn);
                if (conn.State != ConnectionState.Open) conn.Open();
                try {
                    results = command.ExecuteNonQuery();
                } catch (Exception ex) {
                    outcome.Success = false;
                    outcome.Message = "Backup local database failed: database:" + config.DatabaseName;
                    outcome.Techmessage = "Backup local database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                }
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Backup local database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Backup local database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }

        private ReturnClass BackupRemote() {
            ReturnClass outcome = new ReturnClass(true);
            SqlConnection conn = null;
            SqlCommand command = null;
            string Sql = "";
            int results = 0;
            try{
                Sql = "Backup Database " + config.DatabaseName + " to disk='" + config.BACKUPREMOTEA + "' WITH INIT";
                conn = new SqlConnection(config.RemoteConnectionString);
                command = new SqlCommand(Sql, conn);
                if (conn.State != ConnectionState.Open) conn.Open();
                try {
                    results = command.ExecuteNonQuery();
                } catch (Exception ex) {
                    outcome.Success = false;
                    outcome.Message = "Backup remote database failed: database:" + config.DatabaseName;
                    outcome.Techmessage = "Backup remote database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                }
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Backup remote database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Backup remote database failed: database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }

        private ReturnClass CopyLocalToRemote() {
            FileInfo Localfile = null;
            ReturnClass outcome = new ReturnClass(true);
            try {
                Localfile = new FileInfo(config.BACKUPLOCAL2);
                Localfile.CopyTo(config.BACKUPREMOTEB, true);
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Copy local backup to remote backup failed! Database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Copy local backup to remote backup failed! Database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }

        private ReturnClass CopyRemoteToLocal() {
            FileInfo Localfile = null;
            ReturnClass outcome = new ReturnClass(true);
            try {
                Localfile = new FileInfo(config.BACKUPREMOTEB);
                Localfile.CopyTo(config.BACKUPLOCAL2, true);
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Copy remote backup to local backup failed! Database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Copy remote backup to local backup failed! Database:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }

        private ReturnClass RestoreRemoteServer() {
            ReturnClass outcome = new ReturnClass(true);
            SqlConnection conn = null;
            SqlCommand command = null;
            string Sql = "";
            int results = 0; 
            Sql = "USE MASTER ";
            Sql += "RESTORE DATABASE " + config.DatabaseName + " " +
             " FROM DISK = '" + config.BACKUPREMOTEA + "' " +
             " WITH REPLACE," +
             " MOVE '" + config.DatabaseName + "' TO '" + config.REMOTEMDF + "', " +
             " MOVE '" + config.DatabaseName + "_log' TO '" + config.REMOTELDF + "'";

            conn = new SqlConnection(config.RemoteConnectionString);
            command = new SqlCommand(Sql, conn);
            if (conn.State != ConnectionState.Open) conn.Open();
            try {
                results = command.ExecuteNonQuery();
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Restore remote database failed:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Restore remote database failed:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }

        private ReturnClass RestoreLocalServerFromA() {
            ReturnClass outcome = new ReturnClass(true);
            SqlConnection conn = null;
            SqlCommand command = null;
            string Sql = "";
            int results = 0;
            Sql = "USE MASTER ";
            Sql += "RESTORE DATABASE " + config.DatabaseName + " " +
             " FROM DISK = '" + config.BACKUPLOCAL1 + "' " +
             " WITH REPLACE," +
             " MOVE '" + config.DatabaseName + "' TO '" + config.LOCALMDF + "', " +
             " MOVE '" + config.DatabaseName + "_log' TO '" + config.LOCALLDF + "'";

            conn = new SqlConnection(config.RemoteConnectionString);
            command = new SqlCommand(Sql, conn);
            if (conn.State != ConnectionState.Open) conn.Open();
            try {
                results = command.ExecuteNonQuery();
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Restore local database failed:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Restore local database failed:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }

        private ReturnClass RestoreLocalServerFromB() {
            ReturnClass outcome = new ReturnClass(true);
            SqlConnection conn = null;
            SqlCommand command = null;
            string Sql = "";
            int results = 0;
            Sql = "USE MASTER ";
            Sql += "RESTORE DATABASE " + config.DatabaseName + " " +
             " FROM DISK = '" + config.BACKUPLOCAL2 + "' " +
             " WITH REPLACE," +
             " MOVE '" + config.DatabaseName + "' TO '" + config.LOCALMDF + "', " +
             " MOVE '" + config.DatabaseName + "_log' TO '" + config.LOCALLDF + "'";

            conn = new SqlConnection(config.RemoteConnectionString);
            command = new SqlCommand(Sql, conn);
            if (conn.State != ConnectionState.Open) conn.Open();
            try {
                results = command.ExecuteNonQuery();
            } catch (Exception ex) {
                outcome.Success = false;
                outcome.Message = "Restore local database failed:" + config.DatabaseName + " Error:[" + ex.Message + "]";
                outcome.Techmessage = "Restore local database failed:" + config.DatabaseName + " Error:[" + ex.Message + "]";
            }
            return outcome;
        }


        //private ReturnClass SendMSMQMessage(int rowid, string msg, string logfolder, bool success, string queuepath) {
        //    ReturnClass outcome = new ReturnClass(true);
        //    MessageQueue MyMessageQ = null;
        //    Message MyMessage = null;
        //    DatabaseMessageObject obj = null;
        //    try {
        //        MyMessageQ = new MessageQueue(queuepath);
        //    } catch (Exception ex) {
        //        outcome.Success = false;
        //        outcome.Techmessage = "error accessing MessageQueue:" + ex.Message;
        //    }

        //    obj = new DatabaseMessageObject(rowid, success, msg);
        //    try {
        //        MyMessage = new Message();
        //        MyMessage.UseDeadLetterQueue = true;
        //        MyMessage.Formatter = new System.Messaging.BinaryMessageFormatter();
        //        MyMessage.Body = obj;
        //    } catch (Exception ex) {
        //        outcome.Success = false;
        //        outcome.Techmessage = "error creating Message:" + ex.Message;
        //    }
        //    try {
        //        MyMessageQ.Send(MyMessage, "Database Backup Application");
        //    } catch (Exception ex) {
        //        outcome.Success = false;
        //        outcome.Techmessage = "error sending Message:" + ex.Message;
        //    }

        //    return outcome;
        //}
    }

    public class DBConfigClass {
        public int ID = 0;
        public string Title = "";
        public string DatabaseName = "";
        public string LocalConnectionString = "";
        public string RemoteConnectionString = "";
        public string BACKUPLOCAL1 = "";    // D:\folder\subfolder1
        public string BACKUPLOCAL2 = "";    // D:\folder\subfolder2
        public string BACKUPREMOTEA = "";   // \\Server\share1
        public string BACKUPREMOTEB = "";   // E:\folder\subfolder1
        public string LOCALMDF = "";
        public string LOCALLDF = "";
        public string REMOTEMDF = "";
        public string REMOTELDF = "";
        public string ContactName = "";
        public string ContactEmail = "";
        
        public DBConfigClass() { 
        
        }
    }

}
