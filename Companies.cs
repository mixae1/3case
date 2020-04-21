using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SocNetParser
{
    //вынести в отдельный файл
    class Companies
    {
        /// <summary>
        /// получаем список компаний определенного бизнес сектора 
        /// TODO : сделать для любого бизнеса
        /// </summary>
        /// <returns></returns>
        public static List<Company> GetCompanies()
        {//13 мс

            List<Company> Comps = new List<Company>();
           

                WebRequest webr = WebRequest.Create(ParserINN.RND_COMPS_URL);// 1/2 мс
                WebResponse webresp = webr.GetResponse();//40-50 мс
                var t = webresp.GetResponseStream();//620-655мс
                var temp = new StreamReader(t);//2мс
                var comps = Regex.Matches(temp.ReadToEnd(), @"<div class=\Dorg\D>([\s\S]+?)<div class=\Dto-top\D>");//2-5 мс
                foreach (Match x in comps)//16мс в 1 раз 
                {
                   HashSet<string> ad = new HashSet<string>();
               
                var tmp  = SiteParser.adress.Matches(x.ToString());//1-2
                ad = new HashSet<string>();
                foreach (var xx in tmp)
                    ad.Add(xx.ToString());

                var address = ad;
                ad = new HashSet<string>();
                tmp = SiteParser.mail.Matches(x.Value);//1-2

                foreach (var xx in tmp)
                    ad.Add(xx.ToString());
                // Console.WriteLine(string.IsNullOrEmpty(adrs));

                string name = Regex.Match(x.Value, @"<a class=\Dlnk\D[\s\S]+?>([\s\S]+?)<\/a>").Groups[1].Value;//2-3мс
                    Comps.Add(new Company(name,address,ad));//3-4 мс
                }

            return Comps;
        }
      

    }
}
