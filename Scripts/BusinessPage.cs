using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.IO;


namespace SocNetParser
{

    /// <summary>
    /// страница для каждого бизнеса с сайта , нужно для многопоточности и асинхроности 
    /// </summary>

    class BusinessPage
    {

        public string Name { get; }
        public List<Company> comps { get; }

        private static WebClient webclient = new WebClient();

      ///получаем ссылки на вк 
        public List<string> GetVkUrls()
        {
             return comps.Select(x => x.Vk).ToList(); 
        }

        /// <summary>
        /// получаем список сайтов оргов
        /// </summary>
        /// <returns></returns>
        public List<string> GetDomains()
        {
            return comps.Select(x => x.Website).ToList();
        }

        public HashSet<string> GetCompNames()
        {
            return comps.Select(x => x.name).ToHashSet();
        }


        /// <summary>
        /// получаем список сcылок на инсту
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pathToJson"></param>
        public List<string> GetInsTurls()
        {
            return comps.Select(x => x.inst).ToList();
        }


        public BusinessPage(string name,string pathToJson)
        {
            Name = name;
            comps = GetCompaniesJson(pathToJson);
        }




        /// <summary>
        /// подгружаем данные с json файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Company> GetCompaniesJson(string path)
        {
            return JsonSerializer.Deserialize<List<Company>>(File.ReadAllText(path));
        }

        /// <summary>
        /// основной метод дополнения инфы обходит сайты оргов
        /// </summary>
        public  void AddCompanyInfo()
        {
           
            foreach (var tmp in comps)
            {
                if (tmp.socials != null)
                {
                    tmp.Vk = tmp.socials.Where(x => x.Contains("vk.com")).FirstOrDefault();
                    tmp.inst = tmp.socials.Where(x => x.Contains("inst")).FirstOrDefault();
                }
                
                tmp.Website = tmp.webs == null ? null : tmp.webs.FirstOrDefault();
                tmp.adress.Add(tmp.adressFromJson);
                

                if (string.IsNullOrEmpty(tmp.Website)) continue;
                {
                    //Console.WriteLine(tmp.Vk);
                    Console.WriteLine("here");
                    AddInfo(tmp);
                }
          
            }
        }
        /// <summary>
        /// дополнительный метод заходит на сайт конкретной организации и получает инфу
        /// </summary>
        /// <param name="comp"></param>
         void AddInfo(Company comp)
        {

            string s;

            try
            {
                s = webclient.DownloadString(comp.Website);
            }
            catch
            {
                return;
            }



            var phns = SiteParser.phone.Matches(s);
            if (comp.phones == null) comp.phones = new List<string>();
            foreach (var x in phns)
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
                if (comp.Vk == null)
                {   
                    
                    Console.WriteLine("!!!!!");
                    comp.Vk = vk.Value;
                    Console.WriteLine(comp.Vk);
                }
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
            if (comp.adress == null) comp.adress = new List<string>();
            foreach (var x in adrs)
            {
                comp.adress.Add(x.ToString());
            }


         }



    }
}
