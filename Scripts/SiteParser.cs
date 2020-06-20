using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
namespace SocNetParser
{
    

    static class SiteParser
    {
        public static Regex face = new Regex(@"facebook\.com\/\w+");
        public static Regex inst = new Regex(@"instagram\.com\/\w+");
        public static Regex vk = new Regex(@"vk\.com\/\w+");
        public static Regex Vk = new Regex(@"vk\.com\/[\w\.]+");
        public static Regex mail = new Regex(@"\w[\w\d_\-]{0,15}\@[\w\-]{2,15}\.\w{1,3}");
        public static Regex phone = new Regex(@"\D(?<num>\+?[78]\ ?\(?\d{3}\)?\ ?\d{3}([ \-]?)\d{2}\1\d{2})\D");
        public static Regex adress = new Regex(@"[^>\n]{0,50}(улица|ул\.|проспект|просп\.|переулок|пер\.|д\.)[^<\n]{0,50}");
        public static Regex websites = new Regex(@"\D?https?:\/\/([а-яА-Яa-zA-Z0-9]\.?\-?)+\.\w{1,4}\D?");


        //регулярки для сайта whois.ru (вернее для ответа от апишки этого сервиса)
        public static Regex CheckAvail = new Regex(@"available\D:\D(no)\D");
        public static Regex RusDomainCr = new Regex(@"created\s*:\s*(\d{4}-\d{2}-\d{2})");
        public static Regex RusDomianTill = new Regex(@"paid-till\s*:\s*(\d{4}-\d{2}-\d{2})");
        public static Regex EngDomainCr = new Regex(@"Creation Date\s*:\s*(\d{4}-\d{2}-\d{2})");
        public static Regex EngDomainUpd = new Regex(@"Updated Date\s*:\s*(\d{4}-\d{2}-\d{2})");
        public static Regex EngDomainExpr = new Regex(@"Expiration Date\s*:\s*(\d{4}-\d{2}-\d{2})");




        public static HashSet<string> SiteSearching(string site)
        {
            //File.WriteAllText("t2.txt", File.ReadAllText("ris.html"));
            //https://rris.ru/rostov-na-donu-001/ some troubles

            WebClient webclient = new WebClient();
            webclient.Headers.Add(HttpRequestHeader.UserAgent, "");
            
            //Разные варианты загрузки html страницы
            //1
            var buf = webclient.DownloadString(site);

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


    }
}
