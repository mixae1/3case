using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using VkNet;
using System.Runtime.InteropServices;
using SocNetParser.Properties;
using System.Text.Json;

namespace SocNetParser
{
    class MainProgram
    {

        public static VKParser vkApi = new VKParser();
        static void Main()
        {
            //òåñò 
           
           var t = new BusinessPage("pubs", "../../../Data.json");

        //   t.AddCompanyInfo();


            var linqer = new Linqer(t);

            foreach (var tr in linqer.UploadInstFile())
                Console.WriteLine(tr);
        // linqer.StartLinq();
            Console.WriteLine("//////////");
           
           
          //  linqer.LoadToDB();
            
            Console.WriteLine("finished");
        }
    }
}