using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CommonTranslate
{
    public static class LangJson
    {
        private static void println(string text)
        {
            Console.WriteLine(text);
        }

        public static void PrintJObject(JObject jobject)
        {
            foreach (var item in jobject)
            {
                println(item.ToString());
            }
        }
        public static JObject TranslateLangJson(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            JObject translatedJsonObject = new JObject();
            int total = jsonObject.Count;
            int progress = 0;
            println($"[预计用时: >={0.2 * total} 秒]");
            foreach (var item in jsonObject)
            {
                progress++;
                string key = item.Key.ToString();
                string value = item.Value.ToString();
                string translatedValue = TencentTranslate.TranslateEn2Cn(value);
                Thread.Sleep(200);
                println($"[{progress}/{total}]原文:{value},译文:{translatedValue}");
                translatedJsonObject.Add(key, translatedValue);
            }
            return translatedJsonObject;
        }
    }


    

}
