using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Cpp2Lua
{
    class Program
    {
        public static void Main(string[] args)
        {
            string inputFile = @"F:\Workspace\Cpp2Lua\CommStruct.h";
            string outputFile = @"F:\Workspace\Douwan_proj\client\trunk\Assets\GameFramework\Lua\Message\MessageDef.lua";
            Console.WriteLine("args=" + args.Length);

            string protocolDir = @"F:\Workspace\Douwan_proj\monitor\ClientProtocols\";

            if (args == null || args.Length < 2)
            {
                args = new string[] { protocolDir + "BaseDataStruct.h", 
                    protocolDir + "ConfigStruct.h", 
                    protocolDir + "BaseCommonStruct.h",
                    protocolDir + "UserDataStruct.h",
                    protocolDir + "UserMiscelCommonStruct.h",
                    protocolDir + "UserBaseCommonStruct.h",
                    outputFile };
            }            
            
            for (int i = 0; i < args.Length; i ++)
            {
                Console.WriteLine(args[i]);
            }

            Reader reader = new Reader();
            ArrayList list = new ArrayList();
            for (int i = 0; i < args.Length - 1; i ++ )
            {
                Console.WriteLine("[Program.cs]Process File:" + args[i]);
                list.AddRange(reader.ReadNormalHeadFile(args[i]));
            }

            Writer writer = new Writer();
            writer.WriteLuaDefineFile(list, args[args.Length - 1]);

            int h = 0;
            Console.Write(h);
        }
    }
}
