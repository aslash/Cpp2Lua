using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cpp2Lua
{
    public class Reader
    {
	public void Test2Func()
	{
	}

	public void BranchFunc()
	{
	}
	public void Test()
	{
	    byte[] byte = new byte[10];
	}
        // 仅仅包含结构定义的文件
        public void ReadPureStructDef()
        {           
            /*string msg = "okffffffffffffffff";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(msg);  //转换为字节
            using (FileStream fsWrite = new FileStream(@"D:\1.txt", FileMode.Create))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };*/

            FileStream readerStream = new FileStream(@"F:\Workspace\Cpp2Lua\def.h", FileMode.Open);
            int fsLen = (int)readerStream.Length;
            byte[] heByte = new byte[fsLen];
            int r = readerStream.Read(heByte, 0, heByte.Length);
            
            string myStr = System.Text.Encoding.UTF8.GetString(heByte);
            myStr = Util.TrimComments(myStr);

            myStr = myStr.Replace(Environment.NewLine, "").Replace("typedef", "");
            string[] strArr = myStr.Split(new string[] { "struct"}, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strArr.Length; i ++ )
            {
                if (strArr[i].Length <= 3)
                {
                    continue;
                }
                Struct str = new Struct();
                str.Construct(strArr[i]);
                Console.WriteLine(str.name);
            }
            Console.WriteLine(myStr);
            Console.ReadKey();
        }

        public ArrayList ReadNormalHeadFile(string file, bool included = false)
        {
            FileStream readerStream = new FileStream(file, FileMode.Open);
            int fsLen = (int)readerStream.Length;
            byte[] heByte = new byte[fsLen];
            int r = readerStream.Read(heByte, 0, heByte.Length);
            
            string myStr = System.Text.Encoding.UTF8.GetString(heByte);
            // myStr = Util.TrimComments(myStr);

            ReadDefineValue(myStr);
            
            ArrayList structList = new ArrayList();
            while (myStr.Contains("struct"))
            {
                // 查找到第一个 struct
                int startIndex = myStr.IndexOf("struct");

                // 去掉第一个 struct 之前的所有字符
                myStr = myStr.Remove(0, startIndex);

                // 查找 struct 之后的第一个 }
                int tempIndex = myStr.IndexOf("}");

                char firstAfter = ' ';
                int firstAfterIndex = 0;
                // 找到 '}' 后的第一个非空字符
                for (int i = tempIndex + 1;i < myStr.Length; i ++)
                {
                    if (myStr[i] == ' ' || myStr[i] == '\t')
                    {
                        continue;
                    }

                    firstAfter = myStr[i];
                    firstAfterIndex = i;
                    break;
                }

                string structStr = "";
                // 结构体名在前面的情况
                if (firstAfter == ';')
                {                    
                    structStr = myStr.Substring(0, tempIndex + 1);                                      
                }
                else
                {
                    string subStr = myStr.Substring(firstAfterIndex, myStr.Length - firstAfterIndex);
                    int firstCharIndex = subStr.IndexOf(";");
                    int realCharIndex = firstAfterIndex + firstCharIndex;
                    int lastIndex = myStr.LastIndexOf(";");
                    structStr = myStr.Substring(0, realCharIndex);
                }

                structStr = structStr.Replace("struct", "");
                structStr = Util.TrimComments(structStr);

                structStr = Util.TrimNewline(structStr).Replace("typedef", "");
                string[] strArr = structStr.Split(new string[] { "struct" }, StringSplitOptions.RemoveEmptyEntries);

                Struct str = new Struct();
                str.Construct(structStr, file, included);                              
                myStr = myStr.Remove(0, tempIndex);   
             
                if (Util.IsIgnoreStruct(str.name) == false && str.unexported == false)
                {
                    structList.Add(str);
                }                

                Console.WriteLine("[Reader.cs]" + str.name);
            }

            for (int i = 0; i < structList.Count; i ++)
            {
                Struct stru = structList[i] as Struct;                
            }

            return structList;
        }

        public void ReadDefineValue(string str)
        {
            while (str.Contains("#define"))
            {
                int startIndex = str.IndexOf("#define");
                str = str.Remove(0, startIndex);
                int lastIndex = str.IndexOf(Environment.NewLine);
                if (lastIndex == -1)
                {
                    lastIndex = str.IndexOf("\n");
                }

                string defineStr = str.Substring(0, lastIndex);
                defineStr = defineStr.Replace("\t", " ");

                string[] arr = defineStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length < 3)
                {
                    str = str.Remove(0, lastIndex);
                    continue;
                }

                string key = arr[1].Trim();
                string value = arr[2].Trim();

                int val;
                if (int.TryParse(value, out val))
                {
                    DefineValue.PutDefineValue(key, val);
                }                
                str = str.Remove(0, lastIndex);
            }
        }
    }
}
