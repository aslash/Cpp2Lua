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
           
            string includeFile = "";
            string inputFile = @"F:\Workspace\Cpp2Lua\CommStruct.h";
            //string outputFile = @"F:\Workspace\Douwan_proj\client\trunk\Assets\GameFramework\Lua\Message\MessageDef.lua";
            Console.WriteLine("args=" + args.Length);

            //string protocolDir = @"F:\W  orkspace\Douwan_proj\monitor\ClientProtocols\";
            string protocolDir = @"F:\Workspace\Douwan_proj\games\FightLandbord_1\ClientProtocols\";
            string outputFile = @"F:\Workspace\Douwan_proj\client\trunk\Assets\GameFramework\Lua\DouWan\DouDiZhu\CoinGame\Message\DDZMessage.lua";
            if (args == null || args.Length == 0)
            {
                /*args = new string[] { "", protocolDir + "BaseDataStruct.h-" +  
                    protocolDir + "ConfigStruct.h-" +
                    protocolDir + "BaseCommonStruct.h-" +
                    protocolDir + "UserDataStruct.h-" +
                    protocolDir + "UserMiscelCommonStruct.h-" +
                    protocolDir + "UserBaseCommonStruct.h-" +
                    protocolDir + "CrossHouseCardFactory.h",
                    outputFile,
                    ""};
                */
                args = new string[] { "", protocolDir + "BaseDataStruct.h-" +  
                    protocolDir + "ConfigStruct.h-" +
                    /*protocolDir + "CrossHouseCardFactory.h-" +*/
                    protocolDir + "UserDataStruct.h-" +
                    protocolDir + "UserBaseCommonStruct.h",
                    outputFile,
                    "DDZ"};
            }
            else if (args.Length < 4)
            {
                Console.WriteLine("[Error]参数列表不正确，应该为4个参数");
                Console.WriteLine("[Error]参数格式：includeFiles inputFiles outputFiles namePrefix");
                return;
            }

            for (int i = 0; i < args.Length; i ++)
            {
                Console.WriteLine(args[i]);
            }

            Reader reader = new Reader();
            ArrayList list = new ArrayList();

            string[] includeFiles = args[0].Split('-');
            DefineValue.CUSTOM_PREFIX = args[3];
            Console.WriteLine("INCLUDE FILE SIZE=" + includeFiles.Length);
            for (int i = 0; i < includeFiles.Length; i++)
            {
                if (includeFiles[i] == "")
                {
                    continue;
                }
                Console.WriteLine("[Program.cs]Process Include File:" + includeFiles[i]);
                list.AddRange(reader.ReadNormalHeadFile(includeFiles[i], true));
            }

            string[] files = args[1].Split('-');
            Console.WriteLine("FILE SIZE=" + files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine("[Program.cs]Process File:" + files[i]);
                list.AddRange(reader.ReadNormalHeadFile(files[i], false));
            }

            Writer writer = new Writer();
            string output = args[2];
            writer.WriteLuaDefineFile(list, output);

            int h = 0;
            Console.Write(h);
        }
    }
}
