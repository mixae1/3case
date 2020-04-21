using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace SocNetParser
{
    class MainProgram
    {  

        static  async void GetData()
        {
              HttpClient h = new HttpClient();
            HttpResponseMessage t = await h.GetAsync(Companies.RND_COMPS_URL);
            string inp = await t.Content.ReadAsStringAsync();
            Console.WriteLine(inp);
        }


        static async Task Main()
        {
            /* 
             System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
             sw.Start();
             var t = Companies.GetCompanies();
             sw.Stop();

             Console.WriteLine(sw.ElapsedMilliseconds);
             ParserINN.PrintCompanyInfo(t);
             */
            HttpClient h = new HttpClient();
            HttpResponseMessage t = await h.GetAsync(Companies.test);
            string inp = await t.Content.ReadAsStringAsync();
            Console.WriteLine(inp);
            
          
            /*sy
              WebRequest webr = (HttpWebRequest)WebRequest.Create(Companies.RND_COMPS_URL);
              WebResponse webresp = (HttpWebResponse)webr.GetResponse();
              var t = webresp.GetResponseStream();
              var temp = new StreamReader(t);
              Console.WriteLine(temp.ReadToEnd());
           //   var tempp = Companies.websites.Matches(temp.ReadToEnd());
             // foreach (Match s in tempp)
               //   Console.WriteLine(s.ToString());
              */
        }
    }


}
