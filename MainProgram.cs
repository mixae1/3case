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



        public static bool AppAdress(string adress, string[] streets)
        {
            foreach (var x in streets)
                if (adress.Contains(x)) return true;
            return false;

        }


        

        static void  Main()
        {
            VKParser vk = new VKParser();
            Console.WriteLine(vk.GetCountOfPosts(new VKParser.ParserParams("reddit")));
          
            var t =(File.ReadAllLines("CentralStreets.txt"));
           

            var cafees = new BusinessPage(SitesData.RND_COMPS_URL + "kafe");
            if (cafees.webName == null)
                Console.WriteLine("blyat");
            else
            {
                Companies.GetCompanies(cafees, x => AppAdress(x, t));
                var temp = cafees.comps;
                Companies.PrintCompanyInfo(temp);
            }


              //Console.WriteLine(sw.ElapsedMilliseconds);
            // Companies.PrintCompanyInfo(t);
            // Console.WriteLine(t[0].phones[0]);


            //  var t = Companies.GetCompanies();
         /*   
            WebClient webclient = new WebClient();

            webclient.DownloadFile(SitesData.RND_COMPS_URL + "kafe", "buf1.txt");
            webclient.DownloadFile(SitesData.RND_COMPS_URL + "kafe/page-2/", "buf2.txt");



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