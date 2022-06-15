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

        public static JObject TranslateLangJsonNameDescription(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            JObject translatedObject = new JObject();
            int total = 0;
            foreach (var item in jsonObject)
            {
                JObject itemObject = JObject.Parse(item.Value.ToString());
                JArray descriptionArray = JArray.Parse(itemObject["description"].ToString());
                total += 1+ descriptionArray.Count;
            }
            int progress = 0;
            println($"[预计用时: >={0.2 * total} 秒]");
            foreach (var item in jsonObject)
            {
                progress++;
                JObject itemObject = JObject.Parse(item.Value.ToString());
                JObject translatedItemObject = new JObject();
                JArray descriptionArray = JArray.Parse(itemObject["description"].ToString());
                string name = itemObject["name"].ToString();

                Thread.Sleep(200);
                string translatedName = TencentTranslate.TranslateEn2Cn(name);
                println($"[{progress}/{total}]原文:{name},译文:{translatedName}");
                translatedItemObject.Add("name", translatedName);

                JArray translatedArray = new JArray();
                foreach (var description in descriptionArray)
                {
                    progress++;
                    Thread.Sleep(200);
                    string translatedDescription = TencentTranslate.TranslateEn2Cn((string)description);
                    println($"[{progress}/{total}]原文:{(string)description},译文:{translatedDescription}");
                    translatedArray.Add(translatedDescription);
                }
                translatedItemObject.Add("description", translatedArray);
                translatedObject.Add(item.Key.ToString(), translatedItemObject);
            }
            return translatedObject;
        }
    }


    

}
