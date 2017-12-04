using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpp2Lua
{
    class DefineValue
    {
        // 宏定义常亮
        static Dictionary<string, int> defineValue = new Dictionary<string, int>();  
     
        // 自定义的结构类型及其长度，保存下来给嵌套结构体使用
        static Dictionary<string, int> structSize = new Dictionary<string, int>();

        static Dictionary<string, int> structRefCount = new Dictionary<string, int>();

        public static void PutDefineValue(string key, int value)
        {
            int val = 0;
            if (defineValue.TryGetValue(key, out val) == false)
            {
                defineValue.Add(key, value);
                // Console.WriteLine(key + "," + value);
            }                                    
        }

        public static int GetDefineValue(string key)
        {
            int value = 0;
            defineValue.TryGetValue(key, out value);
            return value;
        }

        public static void PutStructSize(string key, int size)
        {
            int value = 0;
            if (structSize.TryGetValue(key, out value) == false)
            {
                structSize.Add(key, size);                
            }
        }
        
        public static int GetStructSize(string key)
        {
            int value = 0;
            structSize.TryGetValue(key, out value);
            return value;
        }

        // 增加结构体被引用次数，处理嵌套情况
        public static void AddStructRefCount(string key)
        {
            int value = 0;
            if (structRefCount.TryGetValue(key, out value) == false)
            {
                structRefCount.Add(key, 1);
            }
            else
            {
                structRefCount[key] += 1;
            }
        }

        public static int GetStructRefCount(string key)
        {
            int value = 0;
            structRefCount.TryGetValue(key, out value);
            return value;
        }
    }
}
