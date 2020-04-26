using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
namespace SocNetParser
{
    class Companies
    {

       
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
        public static void FullFillCompanies(BusinessPage bs, Predicate<string> pred, int Compsneeds = 1)
        {//13 мс

            string page = "page-";
             int index = 0;
            var lst = new List<Company>();
            WebClient webclient = new WebClient();
            webclient.Headers.Add(HttpRequestHeader.UserAgent, "");



            for (int i = 1; i < 50; i++)//до 5 потому что пока парсим сайт кафе ,там 58 страниц ,точно ошибки не будет 
            {
                string sourse = bs.webName + "/" + page + i;
                var buf = webclient.DownloadString(sourse);
                var comps = RND_reg.Matches(buf);


                foreach (Match x in comps)//16мс в 1 раз 
                {

                    string webs;
                    var web = websites.Matches(x.Value);
                    if (web.Count == 1) webs = null;
                    else
                        webs = web.Last().Value.Trim();

                   var temp = GetVkFaceInst(webs);

                                          

                    HashSet<string> ad = new HashSet<string>();
                    var tmp = SiteParser.adress.Matches(x.ToString());//1-2
                    ad = new HashSet<string>();
                    foreach (var xx in tmp)
                        ad.Add(xx.ToString().Trim());

                    if (ad.Count!=0 && !pred(ad.First())) continue;

                    var address = ad;
                    ad = new HashSet<string>();
                    tmp = SiteParser.mail.Matches(x.Value);//1-2


                    foreach (var xx in tmp)
                        ad.Add(xx.ToString());




                    List<string> phones = new List<string>();
                    tmp = SiteParser.phone.Matches(x.Value);


                    foreach (Match y in tmp)
                        phones.Add(y.Value.Trim());


                    string name = RND_name.Match(x.Value).Groups[2].Value;//2-3мс

                    lst.Add(new Company(name, address, ad, webs, phones,temp.Item1,temp.Item2,temp.Item3));//3-4 мс
                }

               // Thread.Sleep(500);// чтоб нами не заинтересовались админы сайта 
            }

            bs.comps = lst;
        }



        static (string, string, string) GetVkFaceInst(string website)
        {
            if (website == null) return (null, null, null);
            WebClient wb = new WebClient();
            
            var temp = (string.Empty, string.Empty, string.Empty);
            try
            {
                string tmp = wb.DownloadString(website);
                temp = (SiteParser.vk.Match(tmp).Value, SiteParser.face.Match(tmp).Value, SiteParser.inst.Match(tmp).Value);
            }
            catch (Exception e)
            {
               // Console.WriteLine("no website ot website is incorrect");
            }
            return temp;
        }

        // пусть пока кол-во возвращает ; здесь можно класс прикрутить
        /*
        static int? GetPost(string url)
        {
            if (url == null) return null;

            VKParser vk=new VKParser
        }

    */
        public static bool AppAdress(string adress, string[] streets)
        {
            foreach (var x in streets)
                if (adress.Contains(x)) return true;
            return false;

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
                Console.WriteLine($"face ={temp.face}");
                Console.WriteLine($"inst ={temp.inst}");
                Console.WriteLine($"vk ={temp.vk}");

                Console.WriteLine("//////////");
            }
        }
    }
}
       

 

