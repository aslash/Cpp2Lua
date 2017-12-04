using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpp2Lua
{
    class Item
    {
        public string type;
        public string name;
        public int length;

        public Item()
        {
        }        
        public void Construct(string key, string value)
        {
            type = key.Trim();
            if (value.Contains("[") == false)
            {
                name = value;
                length = 1;
            }
            else
            {
                string[] arr = value.Split(new string[] { "[" }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 2)
                {
                    name = arr[0];

                    string lenStr = "";
                    if (arr[1].Contains("["))
                    {
                        lenStr = arr[1].Replace("[", "");
                    }
                    
                    if (arr[1].Contains("]"))
                    {
                        lenStr = arr[1].Replace("]", "");
                    }

                    int val = 0;
                    if (int.TryParse(lenStr, out val))
                    {
                        length = val;
                    }
                    else
                    {
                        length = DefineValue.GetDefineValue(lenStr);
                    }                    
                }
            }

            if (Util.IsCustomizeType(type))
            {
                DefineValue.AddStructRefCount(type);
            }
        }

        public int GetSize()
        {            
            string tStr = this.type.ToLower();
            if (tStr == "char" || tStr == "uchar")
            {
                return length;
            }

            else if (tStr == "int" || tStr == "uint" || tStr == "float")
            {
                return 4 * length;
            }

            else if (tStr == "ushort" || tStr == "short")
            {
                return 2 * length;
            }
            else
            {
                int size = DefineValue.GetStructSize(type) * length;
                if (size > 0)
                {
                    return size;
                }
                else
                {
                    Console.WriteLine("[Error]Unknow Type:" + type);
                }
            }
            return 0;
        }

        public string GetDefaultValue()
        {
            string tStr = this.type.ToLower();
            if (tStr == "char" || tStr == "uchar")
            {
                if (this.length == 1)
                {
                    return "0";
                }
                else
                {
                    return "\"\"";
                }
            }

            else if (tStr == "int" || tStr == "uint" || tStr == "ushort" || tStr == "float")
            {
                if (this.length > 1)
                {
                    return "{}";
                }
                else
                {
                    return "0";
                }                
            }
            else if (DefineValue.GetStructSize(type) > 0)
            {
                return "nil";
            }

            return "0";
        }

        public string GetReadBufferString()
        {
            StringBuilder sb = new StringBuilder();            
            string tStr = this.type.ToLower();
            bool structArr = false;
            if (tStr == "char" || tStr == "uchar")
            {
                if (length > 1)
                {
                    sb.Append("\tself.").Append(name).Append("=").Append("buffer:ReadString(").Append(length.ToString()).Append(")");
                }
                else
                {
                    sb.Append("\tself.").Append(name).Append("=").Append("buffer:ReadByte()");
                }

            }
            else if (tStr == "int")
            {
                if (length > 1)
                {
                    sb.Append("\tfor i = 1,").Append(length.ToString()).Append(", 1 do").Append(Environment.NewLine);
                    sb.Append("\t\tself.").Append(name).Append("[i]=").Append("buffer:ReadInt();").Append(Environment.NewLine);
                    sb.Append("\tend");
                }
                else
                {
                    sb.Append("\tself.").Append(name).Append("=").Append("buffer:ReadInt()");
                }               
            }
            else if (tStr == "ushort")
            {
                if (length > 1)
                {
                    sb.Append("\tfor i = 1,").Append(length.ToString()).Append(", 1 do").Append(Environment.NewLine);
                    sb.Append("\t\tself.").Append(name).Append("[i]=").Append("buffer:ReadShort();").Append(Environment.NewLine);
                    sb.Append("\tend");
                }
                else
                {
                    sb.Append("\tself.").Append(name).Append("=").Append("buffer:ReadShort()");
                }                
            }
            else if (tStr == "uint")
            {
                if (length > 1)
                {
                    sb.Append("\tfor i = 1,").Append(length.ToString()).Append(", 1 do").Append(Environment.NewLine);
                    sb.Append("\t\tself.").Append(name).Append("[i]=").Append("buffer:ReadUInt32();").Append(Environment.NewLine);
                    sb.Append("\tend");
                }
                else
                {
                    sb.Append("\tself.").Append(name).Append("=").Append("buffer:ReadUInt32()");
                }                
            }
            else if (tStr == "float")
            {
                if (length > 1)
                {
                    sb.Append("\tfor i = 1,").Append(length.ToString()).Append(", 1 do").Append(Environment.NewLine);
                    sb.Append("\t\tself.").Append(name).Append("[i]=").Append("buffer:ReadFloat()").Append(Environment.NewLine);
                    sb.Append("\tend");
                }
                else
                {
                    sb.Append("\tself.").Append(name).Append("=").Append("Buffer:ReadFloat()");
                }                
            }
            else
            {                
                if (length <= 1)
                {
                    int size = DefineValue.GetStructSize(type);
                    if (size > 0)
                    {
                        sb.Append("\tself.").Append(name).Append("=").Append(type).Append(".Create();").Append(Environment.NewLine);
                        sb.Append("\tself.").Append(name).Append(":ReadFromBuffer(buffer)");
                    }
                    else
                    {
                        Console.WriteLine("[Error]Unknow Type:" + type);
                    }
                }
                else
                {
                    structArr = true;
                    sb.Append("\tself.").Append(name).Append(" = {};").Append(Environment.NewLine);
                    sb.Append("\tfor i=1,").Append(length + "").Append(",1 do").Append(Environment.NewLine);
                    sb.Append("\t\tself.").Append(name).Append("[i]=").Append(type).Append(".Create();").Append(Environment.NewLine);
                    sb.Append("\t\tself.").Append(name).Append("[i]:ReadFromBuffer(buffer);").Append(Environment.NewLine);
                    sb.Append("\tend").Append(Environment.NewLine);                    
                }
            }
            if (!structArr)
            {
                sb.Append(";").Append(Environment.NewLine);
            }            
            return sb.ToString();
        }

        public string GetWriteBufferString()
        {
            StringBuilder sb = new StringBuilder();
            int size = DefineValue.GetStructSize(type);
            // 嵌套结构类型
            if (size > 0)
            {
                if (length <= 1)
                {
                    sb.Append("\t").Append("self.").Append(name).Append(":Write();").Append(Environment.NewLine);
                }
                else
                {
                    sb.Append("\tfor i=1,").Append(length).Append(",1 do").Append(Environment.NewLine);
                    sb.Append("\t\tself.").Append(name).Append("[i]").Append(":Write();").Append(Environment.NewLine);
                    sb.Append("\tend").Append(Environment.NewLine);
                }
            }
            else
            {                
                string tStr = this.type.ToLower();
                if (tStr == "char" || tStr == "uchar")
                {
                    if (length > 1)
                    {
                        sb.Append("\tMessageByteBuffer.").Append("WriteBytes(self.").Append(name).Append(", ").Append(length.ToString()).Append(");").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("\tMessageByteBuffer.").Append("WriteByte(self.").Append(name).Append(");").Append(Environment.NewLine);
                    }
                }
                else if (tStr == "int")
                {
                    if (length > 1)
                    {
                        sb.Append("\tfor i = 1, #").Append("self.").Append(name).Append(" do").Append(Environment.NewLine);
                        sb.Append("\t\tMessageByteBuffer.").Append("WriteInt(self.").Append(name).Append("[i]);").Append(Environment.NewLine);
                        sb.Append("\tend").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("\tMessageByteBuffer.").Append("WriteInt(self.").Append(name).Append(");").Append(Environment.NewLine);
                    }
                }
                else if (tStr == "ushort")
                {
                    if (length > 1)
                    {
                        sb.Append("\tfor i = 1, #").Append("self.").Append(name).Append(" do").Append(Environment.NewLine);
                        sb.Append("\t\tMessageByteBuffer.").Append("WriteShort(self.").Append(name).Append("[i]);").Append(Environment.NewLine);
                        sb.Append("\tend").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("\tMessageByteBuffer.").Append("WriteShort(self.").Append(name).Append(");").Append(Environment.NewLine);
                    }
                }
                else if (tStr == "uint")
                {
                    if (length > 1)
                    {
                        sb.Append("\tfor i = 1, #").Append("self.").Append(name).Append(" do").Append(Environment.NewLine);
                        sb.Append("\t\tMessageByteBuffer.").Append("WriteUInt32(self.").Append(name).Append("[i]);").Append(Environment.NewLine);
                        sb.Append("\tend").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("\tMessageByteBuffer.").Append("WriteUInt32(self.").Append(name).Append(");").Append(Environment.NewLine);
                    }
                    
                }
                else if (tStr == "float")
                {
                    if (length > 1)
                    {
                        sb.Append("\tfor i = 1, #").Append("self.").Append(name).Append(" do").Append(Environment.NewLine);
                        sb.Append("\t\tMessageByteBuffer.").Append("WriteFloat(self.").Append(name).Append("[i]);").Append(Environment.NewLine);
                        sb.Append("\tend").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("\tMessageByteBuffer.").Append("WriteFloat(self.").Append(name).Append(");").Append(Environment.NewLine);
                    }                    
                }                
            }
            
            return sb.ToString();
        }        
    }
}
