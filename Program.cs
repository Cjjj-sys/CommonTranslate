using Newtonsoft.Json.Linq;
using System;
using System.Net.Mime;

namespace CommonTranslate;

static class Program
{
    private static void Main(string[] args)
    {
        Console.Title = "CommonTranslate v0.1.0";
        InitTencentTranslateIdKey();
        while (true)
        {
            print("请输入文件路径(或者把文件拖进来,输入;exit退出):");
            string filePath = Console.ReadLine().Replace("\"", "");
            if (filePath == ";exit")
            {
                break;
            }
            else if (Directory.Exists(filePath))
            {
                PrintModesDirectory();
                print("请选择目录的格式(默认为 1):");
                string translateModeDirectory = Console.ReadLine();
                switch (translateModeDirectory)
                {
                    case "1":
                        TranslateBedrockBehaviourItems(filePath);
                        break;
                    default:
                        TranslateBedrockBehaviourItems(filePath);
                        break;
                }
                println("====================================");
                println($"翻译完成,请查看目录 {filePath + ".tanslated"}");
                println("====================================\n");
                continue;
            }
            PrintModes();
            print("请选择文件的格式(默认为 1):");
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
                case "4":
                    TranslateBedrockLang(filePath);
                    break;
                default:
                    TranslateJson(filePath);
                    break;
            }
            println("====================================");
            println($"翻译完成,请查看文件 {filePath+".txt"}");
            println("====================================\n");
        }
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
        println("4.Bedrock Lang 语言文件");
        println("item.name=Name");
        println("  ......");
        println("===============================");
    }

    static void PrintModesDirectory()
    {
        println("===============================");
        println("1.Bedrock Behaviour Pack Items");
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
        JObject langObject = JObject.Parse(langJosn);
        var lang = new Lang() { LangJObject = langObject };
        JObject langJsonObject = lang.TranslateLangJson();
        string targetJson = langJsonObject.ToString();
        File.WriteAllText(filePath + ".txt", targetJson);
    }

    static void TranslateJsonNameDescription(string filePath)
    {
        string langJosn = File.ReadAllText(filePath);
        JObject langJObject = JObject.Parse(langJosn);
        var lang = new Lang() { LangJObject = langJObject };
        JObject langJsonObject = lang.TranslateLangJsonNameDescription();
        string targetJson = langJsonObject.ToString();
        File.WriteAllText(filePath + ".txt", targetJson);
    }

    static void TranslateJsonHQM(string filePath)
    {
        string langJosn = File.ReadAllText(filePath);
        JObject langJsonObject = JObject.Parse(langJosn);
        var langJObject = new Lang() { LangJObject = langJsonObject };
        JArray questsArray = JArray.Parse(langJsonObject["quests"].ToString());
        var langJArray = new Lang() { LangJArray = questsArray };
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

    static void TranslateBedrockLang(string filePath)
    {
        try 
	    {	        
		    List<string> sourceTexts = File.ReadAllLines(filePath).Select(e => e = e.Split('=')[1]).ToList();
            List<string> translatedTexts = new List<string>();
            long total = sourceTexts.Count;
            long progress = 0;
            println($"[预计用时: >={0.2 * total} 秒]");
            foreach (var sourceText in sourceTexts)
            {
                if (sourceText != null)
                {
                    Thread.Sleep(200);
                    string translatedText = TencentTranslate.TranslateEn2Cn(sourceText);
                    Console.WriteLine($"[{progress}/{total}] 原文: {sourceText},译文: {translatedText}");
                    translatedTexts.Add(translatedText);
                    progress++;
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
	    catch (Exception exception)
	    {
            println(exception.Message);
	    }
        
    }

    static void TranslateBedrockBehaviourItems(string directoryPath)
    {
        if (!Directory.Exists($"{directoryPath}.translate"))
        {
            Directory.CreateDirectory($"{directoryPath}.translate");
        }
        string[] fileNames = Directory.GetFiles(directoryPath);
        foreach (string fileName in fileNames)
        {
            Thread.Sleep(200);
            string itemJosn = File.ReadAllText(fileName);
            JObject itemObject = JObject.Parse(itemJosn);
            var lang = new Lang() { LangJObject = itemObject };
            JObject langJsonObject = lang.TranslateItemJsonBedrockBehaviour();
            if (langJsonObject != null)
            {
                string targetJson = langJsonObject.ToString();
                File.WriteAllText($"{directoryPath}.translate/{Path.GetFileName(fileName)}", targetJson);
            }
        }
    }
}
