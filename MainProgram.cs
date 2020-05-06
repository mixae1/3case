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
            var t=Companies.GetCompaniesJson("../../../Data.json");
            Companies.AddCompanyInfo(t);
             Companies.PrintCompanyInfo(t);     
        }
    }
}