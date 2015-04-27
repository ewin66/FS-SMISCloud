using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DTU.Simulator
{
    class MainArgs
    {
        public const string KEY_SERVER_IP = "ServerIP";
        public const string KEY_SERVER_PORT = "ServerPort";

        private Dictionary<string, string> values;

        public string Get(string key, string dv=null)
        {
            return values.ContainsKey(key) ? values[key] : dv;
        }

        public int GetInt(string key, int dv = 0)
        {
            return values.ContainsKey(key) ? Convert.ToInt32(values[key]) : dv;
        }

        private MainArgs()
        {
        }

        public static MainArgs ValueOf(string[] args)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values[KEY_SERVER_IP] = ConfigurationManager.AppSettings[KEY_SERVER_IP];
            values[KEY_SERVER_PORT] =  ConfigurationManager.AppSettings[KEY_SERVER_PORT];
            if (args != null && args.Count() > 0)
            {
                foreach(string s in args) {
                    string[] ss = s.Split('=');
                    if (ss.Length == 2)
                    {
                        string key = ss[0].Trim();
                        string value = ss[1].Trim();
                        values[key] = value;
                    }
                }
            }
            MainArgs a = new MainArgs();
            a.values = values;
            return a;
        }
    }
}
