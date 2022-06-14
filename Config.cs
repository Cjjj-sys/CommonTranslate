using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTranslate
{
    public static class Config
    {
        public const string ConfigPath = "config.json";
        
        public static bool CheckConfigExist()
        {
            bool flag = false;
            if (File.Exists(ConfigPath))
            {
                flag = true;
            }
            return flag;
        }

        public static void InitalizeConfig()
        {
            JObject configJObject = new JObject();
            configJObject.Add("secretId", "");
            configJObject.Add("secretKey", "");
            SaveConfig(configJObject);
        }

        public static void SaveConfig(JObject saveJObject)
        {
            File.WriteAllText(ConfigPath, saveJObject.ToString());
        }

        public static JObject LoadConfig()
        {
            return JObject.Parse(File.ReadAllText(ConfigPath));
        }
    }
}
