using Newtonsoft.Json.Linq;
using System;

namespace CommonTranslate;

static class Program
{
    static void Main(string[] args)
    {
        InitTencentTranslateIdKey();
        bool doExit = false;
        while (!doExit)
        {
            print("请输入文件路径(或者把文件拖进来):");
            string filePath = Console.ReadLine().Replace("\"", "");
            println("请选择文件的格式(默认为 1):");
            PrintModes();
            string translateMode = Console.ReadLine();
            switch (translateMode)
            {
                case "1":
                    TranslateJson(filePath);
                    break;
                case "2":
                    TranslateJsonNameDescription(filePath);
                    break;
                case "3":
                    TranslateJsonHQM(filePath);
                    break;
                default:
                    TranslateJson(filePath);
                    break;
            }
            //TranslateJsonNameDescription(filePath);
            //TranslateJson(filePath);
            println("====================================");
            println($"翻译完成,请查看文件 {filePath+".txt"}");
            println("====================================\n");
        }
        //TranslateMidStr(filePath, "|", "|");
    }

    static void PrintModes()
    {
        println("===============================");
        println("1.普通Json语言文件");
        println("{\n  \"block.one\": \"One Block\",");
        println("  \"item.one\": \"One Item\",");
        println("   ......");
        println("===============================");
        println("2.含Name和Description的Json语言文件");
        println("{\n  \"axe\":{\n    \"name\":\"Axe\",\n    \"description\":[\n      \"Just a Axe.\",\n      \"Nothing Specially.\",\n      \"......\",\n      \":D\"\n    ]\n  },\n  \"apple\":{\n    \"name\":\"Apple\",\n    \"description\":[\n      \"A Megic Apple.\"\n    ]\n  }\n  ......,");
        println("===============================");
        println("3.HQM(Hardcore Questing Mode)任务书Json语言文件");
        println("{\n  \"name\": \"sth\",");
        println("  \"description\": \"sth\",");
        println("  \"quests\": [内含name和description的Array] ,");
        println("  ......");
        println("===============================");
    }

    private static void print(string text)
    {
        Console.Write(text);
    }

    private static void println(string text)
    {
        Console.WriteLine(text);
    }

    static void InitTencentTranslateIdKey()
    {
        if (Config.CheckConfigExist())
        {
            JObject config = Config.LoadConfig();
            string secretId = (string)config["secretId"];
            string secretKey = (string)config["secretKey"];
            if (secretId != "" && secretKey != "")
            {
                TencentTranslate.secretId = secretId;
                TencentTranslate.secretKey = secretKey;
            }
            else
            {
                SaveTencentTranslateIdKey(config);
            }
        }
        else
        {
            Config.InitalizeConfig();
            JObject config = Config.LoadConfig();
            SaveTencentTranslateIdKey(config);
        }
    }

    static void SaveTencentTranslateIdKey(JObject config)
    {
        print("请输入腾讯云SecretId:");
        string secretId = Console.ReadLine();
        print("请输入腾讯云SecretKey:");
        string secretKey = Console.ReadLine();
        string test = "Apple";
        string result = "苹果";
        if (secretId != null && secretKey != null)
        {
            TencentTranslate.secretId = secretId;
            TencentTranslate.secretKey = secretKey;
            if (TencentTranslate.TranslateEn2Cn(test) == result)
            {
                config["secretId"] = secretId;
                config["secretKey"] = secretKey;
                Config.SaveConfig(config);
            }
            else
            {
                throw new Exception(">>> SecretId 或 SecretKey 不正确！<<<");
            }
        }
        else
        {
            throw new Exception(">>> SecretId 或 SecretKey 不能为空！<<<");
        }
    }

    static void TranslateJson(string filePath)
    {
        string langJosn = File.ReadAllText(filePath); 
        var lang = new Lang(langJosn);
        JObject langJsonObject = lang.TranslateLangJson();
        string targetJson = langJsonObject.ToString();
        File.WriteAllText(filePath + ".txt", targetJson);
    }

    static void TranslateJsonNameDescription(string filePath)
    {
        string langJosn = File.ReadAllText(filePath);
        var lang = new Lang(langJosn);
        JObject langJsonObject = lang.TranslateLangJsonNameDescription();
        string targetJson = langJsonObject.ToString();
        File.WriteAllText(filePath + ".txt", targetJson);
    }

    static void TranslateJsonHQM(string filePath)
    {
        string langJosn = File.ReadAllText(filePath);
        JObject langJsonObject = JObject.Parse(langJosn);
        var langJObject = new Lang(langJsonObject);
        JArray questsArray = JArray.Parse(langJsonObject["quests"].ToString());
        var langJArray = new Lang(questsArray);
        JArray translatedArray = langJArray.TranslateLangJsonHQM();
        JObject translatedLangJsonObject = langJObject.TranslateLangJsonHQMRoot();
        translatedLangJsonObject["quests"] = translatedArray;
        string targetJson = translatedLangJsonObject.ToString();
        File.WriteAllText(filePath + ".txt", targetJson);
    }

    static void TranslateMidStr(string filePath, string leftChar, string rightChar)
    {
        List<string> sourceTexts = Tools.GetSourceTexts(filePath, leftChar, rightChar);
        List<string> translatedTexts = new List<string>();
        foreach (var sourceText in sourceTexts)
        {
            if (sourceText != null)
            {
                Thread.Sleep(200);
                string translatedText = TencentTranslate.TranslateEn2Cn(sourceText);
                Console.WriteLine(translatedText);
                translatedTexts.Add(translatedText);
            }
        }
        for (int i = 0; i < sourceTexts.Count; i++)
        {
            string[] fileLines = File.ReadAllLines(filePath);
            for (int j = 0; j < fileLines.Length; j++)
            {
                fileLines[j] = fileLines[j].Replace(sourceTexts[j], translatedTexts[j]);
            }
            File.WriteAllLines(filePath + ".txt", fileLines);
        }
    }
}
