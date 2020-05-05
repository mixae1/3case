using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Text.Json;
using VkNet.Exception;
using System.Net.Http;

namespace SocNetParser
{
    class Companies
    {

        public static string test = "https://rostov-na-donu.bizly.ru/remont-akpp/page-2/";


        static Regex RND_reg = new Regex(@"<div class=\Dtitle\D>([\s\S]+?)<\/ul>");
        static Regex RND_name = new Regex(@"<a href([\s\S])+?>([\s\S]+?)[<\/a>]");


       static WebClient webclient = new WebClient();
      

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
                    var web = SiteParser.websites.Matches(x.Value);
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
        /// получаем список компаний с json файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Company> GetCompaniesJson(string path) 
        { 
            return JsonSerializer.Deserialize<List<Company>>(File.ReadAllText(path));
        }


        public static void AddCompanyInfo(List<Company> lst)
        {
            int counter = 0;
            foreach (var tmp in lst)
            {    counter++;
                if (string.IsNullOrEmpty(tmp.website)) continue;
                AddInfo(tmp);
                Console.WriteLine($"{counter} company from 25 was fullfilled");
               
            }
        }

        static void AddInfo(Company comp)
        {
            
            string ur = "http://";

            string s;

            try
            {
                s = webclient.DownloadString(ur + comp.website);
            }
            catch
            {
                return;
            }

         //   if (s == null) return;

            var phns = SiteParser.phone.Matches(s);
            if (comp.phones == null) comp.phones = new List<string>(); 
            foreach(var x in phns)
            {
                comp.phones.Add(x.ToString());
            }

            var ems = SiteParser.mail.Matches(s);
            if (comp.emails == null) comp.emails = new HashSet<string>();
            foreach (var x in ems)
            {
                comp.emails.Add(x.ToString());
            }

            var vk = SiteParser.vk.Match(s);
            if (vk.Success)
            {
                if ((comp.vk == null) || (comp.vk != null)
                && (!comp.vk.Contains(vk.Value) || !vk.Value.Contains(comp.vk)))
                    comp.vk = vk.Value;
            }

            var face = SiteParser.face.Match(s);
            if (face.Success)
            {
                if ((comp.face == null) || (comp.face != null)
                    && (!comp.face.Contains(vk.Value) || !face.Value.Contains(comp.face)))
                    comp.face = face.Value;
            }

            var inst = SiteParser.inst.Match(s);
            if (inst.Success)
            {
                if ((comp.inst == null) || (comp.inst != null) 
                    && (!comp.inst.Contains(inst.Value) || !inst.Value.Contains(comp.inst)))
                    comp.inst = inst.Value;
            }

            var adrs = SiteParser.adress.Matches(s);
            if (comp.adress == null) comp.adress = new HashSet<string>();
            foreach (var x in adrs)
            {
                comp.adress.Add(x.ToString());
            }


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

                if (temp.adress == null) Console.WriteLine("no adress");
                else 

                 foreach (var tmp in temp.adress)
                  Console.WriteLine(tmp);


                if (temp.emails == null) Console.WriteLine("no phones"); 
                else 
               foreach (var tmp in temp.emails)
                   Console.WriteLine(tmp);

               if (temp.phones.Count > 0)
                 Console.WriteLine(temp.phones[0]);

                Console.WriteLine(temp.website);
                Console.WriteLine(temp.face);
                Console.WriteLine(temp.vk);
                Console.WriteLine(temp.inst);


                Console.WriteLine("//////////");
            }
        }
    }
}
       

 

