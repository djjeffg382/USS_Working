using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MOO
{
    /// <summary>
    /// Class for accessing the global settings stored at C:\MOOGlobalSettings\GlobalSettings.json
    /// </summary>
    public static class Settings
    {
        private const string GLOBAL_SETTINGS_FILE = "C:\\MOOGlobalSettings\\GlobalSettings.json";

        /// <summary>
        /// What type of server this is
        /// </summary>
        public enum ServerClass
        {
            /// <summary>
            /// Production server (ex: Web Server or Jobs Server)
            /// </summary>
            Production,
            /// <summary>
            /// Development server (ex: Personal laptop or dev server)
            /// </summary>
            Development
        }

        private static JObject _SettingsJObj;
        private static JObject SettingsJObj
        {
            get
            {
                if(_SettingsJObj == null)
                {
                    //have not read the settings file yet, do it now
                    string jsonString = File.ReadAllText(GLOBAL_SETTINGS_FILE);
                    _SettingsJObj = JObject.Parse(jsonString);
                    
                    
                }
                return _SettingsJObj;
            }
        }

        /// <summary>
        /// Gets the value from the JSON Settings file by name
        /// </summary>
        /// <param name="Name">The name of the setting</param>
        /// <param name="DefaultVal">Default value if not found</param>
        /// <returns></returns>        
        public static T GetSettingValByName<T>(string Name, object DefaultVal)
        {
            JToken tok = SettingsJObj.SelectToken("$." + Name);
            if (tok == null)
                return (T)Convert.ChangeType(DefaultVal, typeof(T));
            else
            {
                var value = tok.Value<object>();
                return (T)Convert.ChangeType(value, typeof(T));
            }
            
        }


        /// <summary>
        /// The server type 
        /// </summary>
        public static ServerClass ServerType
        {
            get
            {
                return GetSettingValByName<ServerClass>("ServerType",ServerClass.Development);
            }
        }
        #region "Email Settings"

        /// <summary>
        /// The SMTP Server to be used for mailing
        /// </summary>
        public static string SMTP_Server
        {
            get
            {
                return GetSettingValByName<string>("SMTP_Server", "");
            }
        }
        /// <summary>
        /// The from address that will be used for sending email
        /// </summary>
        public static string MailFromAddress
        {
            get
            {
                return GetSettingValByName<string>("MailFromAddress", "");
            }
        }

        /// <summary>
        /// Email address used for priority email settings
        /// </summary>
        public static string PriorityMailFromAddress
        {
            get
            {
                return GetSettingValByName<string>("PriorityMailFromAddress", "");
            }
        }

        /// <summary>
        /// Email for the Process Control Level 2
        /// </summary>
        public static string PC_Level2_Email
        {
            get
            {
                return GetSettingValByName<string>("PC_Level2_Email", "");
            }
        }
        #endregion

        #region "LIMS Settings"

        /// <summary>
        /// The name of the SampleManager Database on MNO-SQL03
        /// </summary>
        public static string LIMS_DB_Name
        {
            get
            {
                string LimsDBName =  GetSettingValByName<string>("LIMS_DB_Name", "");
                if(string.IsNullOrEmpty(LimsDBName))
                    LimsDBName = GetSettingValByName<string>("LIMS_DB_Name_Override", "");

                if (string.IsNullOrEmpty(LimsDBName))
                {
                    if (Settings.ServerType == ServerClass.Production)
                        LimsDBName = "[MNO-LIMS-PROD]";
                    else
                        LimsDBName = "[MNO-LIMS-TEST]";
                }

                return LimsDBName;
            }
        }

        /// <summary>
        /// System Password for LIMS
        /// </summary>
        public static string LIMS_System_Pass
        {
            get
            {
                return GetSettingValByName<string>("LIMS_System_Pass", "");
            }
        }


        /// <summary>
        /// URL for LIMS ML processing
        /// </summary>
        public static string LIMS_ML_Address
        {
            get
            {
                string url =  GetSettingValByName<string>("LIMS_ML_Address", "");
                if (string.IsNullOrEmpty(url))
                {
                    url = "http://mno-dev-lims.mno.uss.com:56113";  //default to dev lims
                    if (MOO.Settings.ServerType == Settings.ServerClass.Production)
                        url = "http://mno-lims.mno.uss.com:56104";
                }
                return url;
            }
        }
        #endregion
    }

}
