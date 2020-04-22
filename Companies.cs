using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;
namespace SocNetParser
{
    //вынести в отдельный файл
    class Companies
    {

        public const string RND_COMPS_URL = "https://rostov-na-donu.bizly.ru/remont-akpp/";
        public static string test = "https://rostov-na-donu.bizly.ru/remont-akpp/";


         static Regex RND_reg = new Regex(@"<div class=\Dtitle\D>([\s\S]+?)<\/ul>");
         static Regex RND_name = new Regex(@"<a href([\s\S])+?>([\s\S]+?)[<\/a>]");

       


   public   static  Regex websites = new Regex(@"\D?http[s]?:\/\/([а-яА-Яa-zA-Z0-9]\.?[-]?)+\.\w{1,4}\D?");

        /// <summary>
        /// получаем список компаний определенного бизнес сектора 
        /// TODO : сделать для любого бизнеса
        /// </summary>
        /// <returns></returns>
        public static List<Company> GetCompanies()
        {//13 мс

            List<Company> Comps = new List<Company>();

            /* TODO :СДЕЛАТЬ МЕТОД ДЛЯ СКАЧИВАНИЯ  САЙТА РОСТОВА 
                WebRequest webr = WebRequest.Create(test);// 1/2 мс
              WebResponse webresp = webr.GetResponse();//40-50 мс
              var t = webresp.GetResponseStream();//620-655мс
            var temp = new StreamReader(t);//2мс
              */
              //var comps = Regex.Matches(temp.ReadToEnd(), @"<div class=\Dorg\D>([\s\S]+?)<div class=\Dto-top\D>");//2-5 мс
                 var comps = RND_reg.Matches(File.ReadAllText("buf.txt"));
          //  var comps = RND_reg.Matches(temp.ReadToEnd());
           Console.WriteLine(comps.Count);

                foreach (Match x in comps)//16мс в 1 раз 
                {
                   HashSet<string> ad = new HashSet<string>();
               
                var tmp  = SiteParser.adress.Matches(x.ToString());//1-2
                ad = new HashSet<string>();
                foreach (var xx in tmp)
                    ad.Add(xx.ToString().Trim());


                var address = ad;
                ad = new HashSet<string>();
                tmp = SiteParser.mail.Matches(x.Value);//1-2


                foreach (var xx in tmp)
                    ad.Add(xx.ToString());
               


                string webs;
                var  web = websites.Matches(x.Value);
                if (web.Count == 1) webs = null;
                else
                    webs =  web.Last().Value.Trim();

                List<string> phones = new List<string>();
                tmp =SiteParser.phone.Matches(x.Value);


                foreach (Match y in tmp)
                    phones.Add(y.Value.Trim());


                string name = RND_name.Match(x.Value).Groups[2].Value;//2-3мс

                    Comps.Add(new Company(name,address,ad,webs,phones));//3-4 мс
                }

            return Comps;
        }




        /// <summary>
        /// метод чисто для проверки ,потом его удалить 
        /// </summary>
        /// <param name="lst"></param>
        public static void PrintCompanyInfo(List<Company> lst)
        {
            foreach (var temp in lst)
            {
                Console.WriteLine(temp.name);

                foreach (var tmp in temp.adress)
                    Console.WriteLine(tmp);

                foreach (var tmp in temp.emails)
                    Console.WriteLine(tmp);

                 if(temp.phones.Count>0)
                    Console.WriteLine(temp.phones[0]);

                Console.WriteLine(temp.website);


                Console.WriteLine("//////////");
            }
        }



    }
}
