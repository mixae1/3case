using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
namespace SocNetParser
{

    //office@autora-rus.ru - доработать регулярку по почтам + 
    //Россия, Ростов-на-Дону, Депутатская улица, 5а - дорабоать регулярку по адресам +

    class SiteParser
    {
        static Regex face = new Regex(@"facebook\.com\/\w+");
        static Regex inst = new Regex(@"instagram\.com\/\w+");
        static Regex vk = new Regex(@"vk\.com\/\w+");
        public static Regex mail = new Regex(@"\w[\w\d_\-]{0,15}\@[\w\-]{4,9}\.\w{1,3}");
        public static Regex phone = new Regex(@"\D(?<num>\+?[78]\ ?\(?\d{3}\)?\ ?\d{3}([ \-]?)\d{2}\1\d{2})\D");
        public static Regex adress = new Regex(@"[^>\n]{0,50}(улица|ул\.|просп\.|пер\.|д\.)[^<\n]{0,50}");

        public static HashSet<string> SiteSearching(string site)
        {
            //File.WriteAllText("t2.txt", File.ReadAllText("ris.html"));
            //https://rris.ru/rostov-na-donu-001/ some troubles

            WebClient webclient = new WebClient();
            webclient.Headers.Add(HttpRequestHeader.UserAgent, "");
            //1
            //var buf = webclient.DownloadString(site);
            
            //2
            //var data = webclient.DownloadData(site);
            //string buf = Encoding.UTF8.GetString(data);
            
            //3
            //web.DownloadFile(site, "buf.txt");
            //var buf = File.ReadAllText("buf.txt");


            HashSet<string> hs = new HashSet<string>();

            var result = face.Match(buf);
            hs.Add(result.Value.TrimStart());
            result = inst.Match(buf);
            hs.Add(result.Value.TrimStart());
            result = vk.Match(buf);
            hs.Add(result.Value.TrimStart());

            var results = mail.Matches(buf);
            foreach (var s in results) hs.Add(s.ToString().TrimStart());
            results = phone.Matches(buf);
            foreach (Match s in results) hs.Add(s.Groups["num"].ToString().TrimStart());
            results = adress.Matches(buf);
            foreach (var s in results) hs.Add(s.ToString().TrimStart());

            return hs; //надо почистить от пустых строк
        }
        /*
        static void Main(string[] args)
        {

            var hs = SiteSearching("https://atlant-don.ru/");
            foreach (var s in hs) Console.WriteLine(s);
        }
        */
    }
}
