using System;
using System.Collections.Generic;
using System.Text;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace SocNetParser
{
    class MainProgram
    {
        static void Main(string[] args)
        {

            VKParser vk = new VKParser();
            Console.WriteLine(vk.GetCountOfPosts(new VKParser.ParserParams("reddit")));

            /*
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var t = Companies.GetCompanies();
            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
            ParserINN.PrintCompanyInfo(t);
            */
   

            /*
              WebRequest webr = WebRequest.Create(ParserINN.RND_COMPS_URL);
              WebResponse webresp = webr.GetResponse();
              var t = webresp.GetResponseStream();
              var temp = new StreamReader(t);
             // Console.WriteLine(temp.ReadToEnd());
              var tempp = Parser.adress.Matches(temp.ReadToEnd());
              foreach (Match s in tempp)
                  Console.WriteLine(s.ToString());
              */
        }
    }


}
