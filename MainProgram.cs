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
            HttpResponseMessage t = await h.GetAsync("https://rostov-na-donu.bizly.ru/avtoatele/");
            string inp = await t.Content.ReadAsStringAsync();
          // var temp=Companies.websites.Matches(inp);
         //   foreach(var x in  temp)
            Console.WriteLine(inp);
        }


        static void  Main()
        {

            
              System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
              sw.Start();
              var t = Companies.GetCompanies();
              sw.Stop();

              Console.WriteLine(sw.ElapsedMilliseconds);
             Companies.PrintCompanyInfo(t);
             Console.WriteLine(t[0].phones[0]);

             

             /*
            var comps =SiteParser.phone.Matches(File.ReadAllText("buf.txt"));
            foreach(Match x in comps)
            Console.WriteLine(x.Value);
            */
            /*
           HttpClient h = new HttpClient();
           HttpResponseMessage t = await h.GetAsync("https://rostov-na-donu.bizly.ru/remont-akpp/");
           string inp = await t.Content.ReadAsStringAsync();
           var temp = SiteParser.phone.Matches(inp);
             foreach(var x in  temp)
           Console.WriteLine(x.ToString());


           WebClient webclient = new WebClient();

           webclient.DownloadFile(Companies.test, "buf.txt");
           var buf = File.ReadAllText("buf.txt");
           Console.WriteLine(buf);
           */

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
