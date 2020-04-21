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
            var api = new VkApi();

            api.Authorize(new ApiAuthParams
            {
                AccessToken = "57d74dc9352faddcb137652ed6c3a1d7c51a28c9bfb02d5c56bbf03bb4fd3eae59cbd49c7c8dbd980b28c"
            });
            Console.WriteLine(api.Token);
            var res = api.Stats.Get(new StatsGetParams({ GroupId = 1 }));

            Console.WriteLine(res.Count);
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
