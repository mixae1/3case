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

namespace SocNetParser
{
    class MainProgram
    {

        public static VKParser vkApi = new VKParser();
        static void Main()
        {
            //òåñò 
            /*
              var t=Companies.GetCompaniesJson("../../../Data.json");
              Companies.AddCompanyInfo(t);
             Companies.PrintCompanyInfo(t);    
           /*
            var t = new VKGroup("club54751973");
            t.LoadPosts(2);
            Console.WriteLine(t.DTofLastTimePosted());
            */

           var t = new BusinessPage("pubs", "../../../Data.json");
           
            t.AddCompanyInfo();
              var linqer = new Linqer(t);
            linqer.StartLinq();
            linqer.PrintBuisnessPage(t);
           // foreach (var temp in t.GetVkUrls())
               // Console.WriteLine(temp);

        }
    }
}