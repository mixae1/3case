using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
namespace SocNetParser
{
    //вынести в отдельный файл
    class Companies
    {

      //  public const string RND_COMPS_URL = "https://rostov-na-donu.bizly.ru/remont-akpp/";
        public static string test = "https://rostov-na-donu.bizly.ru/remont-akpp/page-2/";


        static Regex RND_reg = new Regex(@"<div class=\Dtitle\D>([\s\S]+?)<\/ul>");
        static Regex RND_name = new Regex(@"<a href([\s\S])+?>([\s\S]+?)[<\/a>]");

        // @"<div class=\Dorg\D>([\s\S]+?)<div class=\Dto-top\D>" (2-5) --- РЕГУЛЯРКА К СТАРОМУ САЙТУ ПОКА ПРОШУ НЕ ТРОГАТЬ


        public static Regex websites = new Regex(@"\D?http[s]?:\/\/([а-яА-Яa-zA-Z0-9]\.?[-]?)+\.\w{1,4}\D?");

        /// <summary>
        /// получаем список компаний определенного бизнес сектора 
        /// TODO : сделать для любого бизнеса
        /// </summary>
        /// <returns></returns>
        public static void GetCompanies(BusinessPage bs, Predicate<string> pred, int Compsneeds = 1)
        {//13 мс

            string page = "page-";
            // int index = 1;
            var lst = new List<Company>();
            
            // var comps = RND_reg.Matches(File.ReadAllText("buf1.txt"));
            for (int i = 1; i < 3; i++)
            {
                string sourse = bs.webName + "/" + page + i;
                WebRequest wr = WebRequest.Create(sourse);
                WebResponse wren = wr.GetResponse();
                var temp = new StreamReader(wren.GetResponseStream());
                 var comps = RND_reg.Matches(temp.ReadToEnd());


                foreach (Match x in comps)//16мс в 1 раз 
                {
                    HashSet<string> ad = new HashSet<string>();

                    var tmp = SiteParser.adress.Matches(x.ToString());//1-2
                    ad = new HashSet<string>();
                    foreach (var xx in tmp)
                        ad.Add(xx.ToString().Trim());

                    if (!pred(ad.First())) continue;

                    var address = ad;
                    ad = new HashSet<string>();
                    tmp = SiteParser.mail.Matches(x.Value);//1-2


                    foreach (var xx in tmp)
                        ad.Add(xx.ToString());



                    string webs;
                    var web = websites.Matches(x.Value);
                    if (web.Count == 1) webs = null;
                    else
                        webs = web.Last().Value.Trim();

                    List<string> phones = new List<string>();
                    tmp = SiteParser.phone.Matches(x.Value);


                    foreach (Match y in tmp)
                        phones.Add(y.Value.Trim());


                    string name = RND_name.Match(x.Value).Groups[2].Value;//2-3мс

                    lst.Add(new Company(name, address, ad, webs, phones));//3-4 мс
                }

                Thread.Sleep(1000);// чтоб нами не заинтересовались админы сайта 
            }

            bs.comps = lst;
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

                if (temp.phones.Count > 0)
                    Console.WriteLine(temp.phones[0]);

                Console.WriteLine(temp.website);


                Console.WriteLine("//////////");
            }
        }

      

    }       
} 
       

 

