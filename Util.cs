using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpp2Lua
{
    class Util
    {
        // 服务器自己用，客户端不解析
        public static bool IsIgnoreStruct(string name)
        {
            if (name == "GameUserType")
            {
                return true;
            }

            return false;
        }

        // 去掉字符首尾
        public static string TrimTable(string str)
        {
            while (str.StartsWith("\t"))
            {
                str = str.Substring(1, str.Length - 1);
            }

            while (str.EndsWith("\t"))
            {
                str = str.Substring(0, str.Length - 1);
            }

            return str;
        }

        public static string TrimComments(string str)
        {
            while (str.Contains("//"))
            {                
                int startIndex = str.IndexOf("//");

                string temp = str.Substring(startIndex, str.Length - startIndex);
                int len = temp.IndexOf(Environment.NewLine);
                if (len == -1)
                {
                    len = temp.IndexOf("\n");
                }
                str = str.Remove(startIndex, len);
            }

            while (str.Contains("/*"))
            {
                int startIndex = str.IndexOf("/*");

                int lastIndex = str.IndexOf("*/");

                str = str.Remove(startIndex, lastIndex - startIndex);
            }

            return str;
        }

        // 去掉结尾换行符(可能是 "\n" 或 "\r\n");
        public static string TrimNewline(string str)
        {
            if (str.Contains("\r\n"))
            {
                return str.Replace(Environment.NewLine, "");
            }
            else
            {
                return str.Replace("\n", "");
            }
        }

        // 是不是自定义类型(结构体)
        public static bool IsCustomizeType(string key)
        {
            key = key.ToLower();
            if (key == "char" || key == "uchar" || key == "float" || key == "int" || key == "uint" || key == "short" || key == "ushort")
            {
                return false;
            }

            return true;
        }
    }
}
