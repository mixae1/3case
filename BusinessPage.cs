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

      //получаем ссылки на вк 
        public List<string> GetVkUrls()
        {
             return comps.Select(x => x.Vk).ToList(); 
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

        public  void AddCompanyInfo()
        {
            //  int counter = 0;
            foreach (var tmp in comps)
            {    //counter++;
                if (string.IsNullOrEmpty(tmp.website)) continue;
                AddInfo(tmp);
                //Console.WriteLine($"{counter} company from 25 was fullfilled");
                if (tmp.Vk != null)
                    Console.WriteLine(tmp.Vk);
            }
        }

         void AddInfo(Company comp)
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



    }
}
