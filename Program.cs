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
            TranslateJson(filePath);
            println("====================================");
            println($"翻译完成,请查看文件 {filePath+".txt"}");
            println("====================================\n");
        }
        //TranslateMidStr(filePath, "|", "|");

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
        JObject langJsonObject = LangJson.TranslateLangJson(langJosn);
        string targetJson = langJsonObject.ToString();
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