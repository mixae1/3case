using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json;


namespace SocNetParser
{
    class Companies
    {

        public static string test = "https://rostov-na-donu.bizly.ru/remont-akpp/page-2/";


       static WebClient webclient = new WebClient();
      

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
                if (string.IsNullOrEmpty(tmp.Website))
                    continue;
                 
                AddInfo(tmp);
               Console.WriteLine($"{counter} company from 25 was fullfilled");
                
            }
        }

        static  void AddInfo(Company comp)
        {
            
            string ur = "http://";

            string s;

            try
            {
                s = webclient.DownloadString(ur + comp.Website);
            }
            catch
            {
                return;
            }

     

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

            var vk = SiteParser.Vk.Match(s);
            if (vk.Success)
            {
                if ((comp.Vk == null) || (comp.Vk != null)
                && (!comp.Vk.Contains(vk.Value) || !vk.Value.Contains(comp.Vk)))
                    comp.Vk = vk.Value;
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



        

      static  DateTime GetLastVkPost(string domain)
        {
            if (domain == null) return DateTime.MinValue;
            var t = new VKGroup(domain);
            t.LoadPosts(2);
            return t.DTofLastTimePostedWithWall();
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

                Console.WriteLine(temp.lastVkPost);
                Console.WriteLine(temp.Website);
                Console.WriteLine(temp.face);
                Console.WriteLine(temp.Vk);
                Console.WriteLine(temp.inst);

                Console.WriteLine("creation domain time"+temp.registraionDomain);
                Console.WriteLine("update domain time" + temp.UptDomain);
                Console.WriteLine("expire domain time" + temp.expireDomain);

                Console.WriteLine("//////////");
            }
        }
    }
}
       

 

