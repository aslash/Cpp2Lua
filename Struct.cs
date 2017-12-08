using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace Cpp2Lua
{
    class Struct
    {
        public string fileName;
        public string name;

        // 只是被包括的结构体类型，不生成到文件
        public bool included = false;

        public System.Collections.ArrayList memberList = new System.Collections.ArrayList();

        public void Construct(string s, string file = "", bool incl = false)
        {
            fileName = file;
            included = incl;

            // 构造名字
            s = s.TrimStart().TrimEnd();
            
            if (s[0] == '{')
            {
                name = s.Substring(s.IndexOf("}") + 1, s.Length - s.IndexOf("}") - 1);
                name = name.Replace(";", "");
                name = name.TrimStart().TrimEnd();
            }
            else
            {
                name = s.Substring(0, s.IndexOf("{"));
                name = name.TrimStart().TrimEnd();
            }

            // 构造成员列表
            string str = s.Substring(s.IndexOf("{") + 1, s.IndexOf("}") - s.IndexOf("{") - 1);            
                        
            string[] strList = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strList.Length; i++)
            {
                //指针不处理
                if (str.Contains("*"))
                {
                    continue;
                }

                string member = Util.TrimTable(strList[i]);
                string[] memArr;
                if (member.Contains("\t"))
                {
                    memArr = member.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    memArr = member.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                }

                string key = "", value = "";
                if (memArr.Length == 2)
                {
                    key = memArr[0];
                    value = memArr[1];
                }
                else
                {
                    for (int j = 0; j < memArr.Length; j ++)
                    {
                        if (memArr[j].Contains("\t") == false && memArr[j].Contains(" ") == false)
                        {
                            key = memArr[j];
                            break;
                        }
                    }

                    for (int j = memArr.Length - 1; j >= 0 ;j --)
                    {
                        if (memArr[j].Contains("\t") == false && memArr[j].Contains(" ") == false)
                        {
                            value = memArr[j];
                            break;
                        }
                    }
                }

                Item item = new Item();
                if (key != "" && value != "")
                {
                    item.Construct(key, value);
                    memberList.Add(item);                    
                }                
            }

            DefineValue.PutStructSize(name, int.Parse(this.GetTotalSizeString()));            
        }

        // 构造函数参数列表
        public string GetLuaCtorParam()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < memberList.Count; i++)
            {
                Item item = memberList[i] as Item;
                if (item == null)
                {
                    continue;
                }

                sb.Append(item.name);
                if (i < memberList.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        // 构造函数赋值语句
        public string GetLuaEmptyCtorCode()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < memberList.Count; i++)
            {
                Item item = memberList[i] as Item;
                if (item == null)
                {
                    continue;
                }

                sb.Append("\tself.").Append(item.name).Append("=").Append(item.GetDefaultValue()).Append(";").Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        // 构造函数赋值语句
        public string GetLuaCtorCode()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < memberList.Count; i++)
            {
                Item item = memberList[i] as Item;
                if (item == null)
                {
                    continue;
                }

                sb.Append("\tself.").Append(item.name).Append("=").Append(item.name).Append(";").Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        // 写缓冲的操作串
        public string GetWriteBufferString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < memberList.Count; i++)
            {
                Item item = memberList[i] as Item;
                sb.Append(item.GetWriteBufferString());
            }

            return sb.ToString();
        }

        // 读缓冲的操作串
        public string GetReadBufferString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < memberList.Count; i++)
            {
                Item item = memberList[i] as Item;
                sb.Append(item.GetReadBufferString());
            }

            return sb.ToString();
        }
        // 总字节数的字符串
        public string GetTotalSizeString()
        {
            int size = 0;
            for (int i = 0; i < memberList.Count; i ++)
            {
                size += (memberList[i] as Item).GetSize();
            }

            return size.ToString();
        }

        // 类定义中的成员列表
        public string GetLuaClassDefine()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name).Append(" = {");
            for (int i = 0; i < memberList.Count; i++)
            {
                Item item = memberList[i] as Item;
                if (item == null)
                {
                    continue;
                }

                sb.Append(item.name).Append("=").Append(item.GetDefaultValue());
                if (i < memberList.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("}").Append(Environment.NewLine);

            return sb.ToString();
        } 

        public string GetStructDefineString()
        {
            StringBuilder sb = new StringBuilder();

            fileName = fileName.Substring(fileName.LastIndexOf("\\"), fileName.Length - fileName.LastIndexOf("\\"));
            fileName = fileName.Substring(1, fileName.Length - 1);

            // LoginReq = {username="", passwd=""}
            sb.Append("----------Define for ").Append(name).Append("----(").Append(fileName).Append(")----------").Append(Environment.NewLine);
            sb.Append(GetLuaClassDefine());

            sb.Append(Environment.NewLine);

            // LoginReq.__index = LoginReq
            sb.Append(name).Append(".__index = ").Append(name).Append(Environment.NewLine).Append(Environment.NewLine);

            //--------------------------Req有用部分------------------------------//
            //function LoginReq:New(u, p) 
            sb.Append("----------给服务器发送的响应使用这个构造----------------").Append(Environment.NewLine);
            sb.Append("function ").Append(name).Append(":New(cmd");
            string ctorParamStr = GetLuaCtorParam();
            if (ctorParamStr == null || ctorParamStr.Length > 0)
            {
                sb.Append(",").Append(ctorParamStr);
            }            
            sb.Append(")").Append(Environment.NewLine);

            // local self = {};
            sb.Append("\tlocal self = {};").Append(Environment.NewLine);

            // setmetatable(self, LoginReq);
            sb.Append("\tsetmetatable(self, ").Append(name).Append(");").Append(Environment.NewLine).Append(Environment.NewLine);
            sb.Append("\tself.cmd = cmd;").Append(Environment.NewLine);
            // self.username = u;
            // self.passwd = p;
            sb.Append("\tself.size=").Append(GetTotalSizeString()).Append(";").Append(Environment.NewLine);
            sb.Append(GetLuaCtorCode());

            // return self;    --返回自身
            // end
            sb.Append(Environment.NewLine).Append("\treturn self;").Append(Environment.NewLine).Append("end").Append(Environment.NewLine).Append(Environment.NewLine).Append(Environment.NewLine);

            sb.Append("----------编码----------------").Append(Environment.NewLine);
            // function LoginReq:WriteToBuffer()
            sb.Append("function ").Append(name).Append(":WriteToBuffer()").Append(Environment.NewLine);

            // MessageByteBuffer.CreateWithPackHead(tonumber(MessageType.MSG_TYPE_C_LOGINCHECKUSER), 64 + 8);    
            sb.Append("\tMessageByteBuffer.CreateWithPackHead(self.cmd, ").Append(GetTotalSizeString()).Append(" + 8, GetServerTarget(self.cmd));").Append(Environment.NewLine).Append(Environment.NewLine);

            sb.Append(GetWriteBufferString());
            sb.Append("end").Append(Environment.NewLine).Append(Environment.NewLine);  

            /*-----------------对于被引用的结构体，定义一个不处理头部的写buffer函数 ----------------*/
            if (DefineValue.GetStructRefCount(this.name) > 0)
            {
                sb.Append("function ").Append(name).Append(":Write()").Append(Environment.NewLine);
                sb.Append(GetWriteBufferString());
                sb.Append("end").Append(Environment.NewLine).Append(Environment.NewLine); 
            }
            //----------------------------Rsp有用部分----------------------------------//

            //function LoginRsp:Create() 
            sb.Append("----------服务器来的包用这个构造--------------").Append(Environment.NewLine);
            sb.Append("function ").Append(name).Append(":Create()").Append(Environment.NewLine);            

            // local self = {};
            sb.Append("\tlocal self = {};").Append(Environment.NewLine);

            // setmetatable(self, LoginRsp);
            sb.Append("\tsetmetatable(self, ").Append(name).Append(");").Append(Environment.NewLine).Append(Environment.NewLine);
            sb.Append("\tself.cmd = 0;").Append(Environment.NewLine);
            // self.username = "";
            // self.passwd = 0;
            sb.Append("\tself.size=").Append(GetTotalSizeString()).Append(";").Append(Environment.NewLine);
            sb.Append(GetLuaEmptyCtorCode());

            // return self;    --返回自身
            // end
            sb.Append(Environment.NewLine).Append("\treturn self").Append(Environment.NewLine).Append("end").Append(Environment.NewLine).Append(Environment.NewLine).Append(Environment.NewLine);

            // function LoginRsp:ReadFromBuffer()
            sb.Append("----------解码----------").Append(Environment.NewLine);
            sb.Append("function ").Append(name).Append(":ReadFromBuffer(buffer)").Append(Environment.NewLine).Append(Environment.NewLine);

            
            //sb.Append("--没有用到的clientId和verifyNum").Append(Environment.NewLine);
            //sb.Append("\tbuffer:ReadInt();").Append(Environment.NewLine);
            //sb.Append("\tbuffer:ReadInt();").Append(Environment.NewLine);
            // sb.Append(Environment.NewLine);           
            sb.Append(GetReadBufferString());
            sb.Append("end").Append(Environment.NewLine).Append(Environment.NewLine);
            return sb.ToString();
        }
        public string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name).Append("\n");
            for (int i = 0; i < memberList.Count; i ++)
            {
                Item member = memberList[i] as Item;
                sb.Append(member.name + "|" + member.type + "|" + member.length);
            }

            return sb.ToString();
        }
    }
}
