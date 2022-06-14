using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTranslate
{
    public static class Tools
    {
        private static void println(string text)
        {
            Console.WriteLine(text);
        }
        public static List<string> GetSourceTexts(string filePath, string leftChar, string rightChar)
        {
            string[] originTexts = File.ReadAllLines(filePath);
            List<string> sourceTexts = new List<string>();
            foreach (var originText in originTexts)
            {
                //label_back|Back|
                string tempText = GetMidStr(originText, leftChar, rightChar);
                sourceTexts.Add(tempText);
            }
            return sourceTexts;
        }

        public static string? GetMidStr(string str, string left, string right)
        {
            if (str == null)
            {
                return null;
            }
            else
            {
                string reverseStr = Reverse(str);
                int leftIndex = str.IndexOf(left) + 1;
                int rightIndex = str.Length - reverseStr.IndexOf(right);
                string result = str.Substring(leftIndex, rightIndex - leftIndex - 1);
                return result;
            }
        }

        private static string Reverse(string text)
        {
            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
