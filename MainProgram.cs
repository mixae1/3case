using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace SocNetParser
{
    class MainProgram
    {



        static void Main()
        {
            //тест 
            var t=Companies.GetCompaniesJson("../../../Data.json");
            Companies.AddCompanyInfo(t);
             Companies.PrintCompanyInfo(t);
          

            

        }
    }
}