using System;
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
               
                var tmp  = Parser.adress.Matches(x.ToString());//1-2
                ad = new HashSet<string>();
                foreach (var xx in tmp)
                    ad.Add(xx.ToString());

                var address = ad;
                ad = new HashSet<string>();
                tmp = Parser.mail.Matches(x.Value);//1-2

                foreach (var xx in tmp)
                    ad.Add(xx.ToString());
                // Console.WriteLine(string.IsNullOrEmpty(adrs));

                string name = Regex.Match(x.Value, @"<a class=\Dlnk\D[\s\S]+?>([\s\S]+?)<\/a>").Groups[1].Value;//2-3мс
                    Comps.Add(new Company(name,address,ad));//3-4 мс
                }

            return Comps;
        }
      

    }

    /// <summary>
    /// класс 
    /// </summary>

    class Company
    {
      public   string name { get; set; }
      //  string inn { get; set; }

      //список физических или юридических адресов
      public   HashSet<string> adress { get; set; }
     
       
        public HashSet<string> emails { get; set; }
     
        public Company(string name, HashSet<string> adress,HashSet<string> hs)
        {
            this.adress = adress;
            emails = hs;
            this.name = name;
          
        }
    }



    //пусть пока будет интернал ,потом разберемся ,наверное вообще статичным сделаем 
    class ParserINN
    {   /// <summary>
    /// ссылка для запроса к сайту ,где инн берем
    /// </summary>
      const string LISTORG_URL= "https://www.list-org.com/search?type=address&val=";
        /// <summary>
        /// ссылка для запроса к основному сайту (пока ведет к авто тематике)
        /// </summary>
      public  const string RND_COMPS_URL = "https://rostov-na-donu.jsprav.ru/avtosalonyi/";


        //подготовка адреса сайта к запросу
       public  static string GetСorrectAdress(string OrgAddress)
        {
            OrgAddress = Regex.Replace(OrgAddress, @"\s+", @"+");
            OrgAddress = Regex.Replace(OrgAddress, @",", @"%2F"); // 
            OrgAddress = Regex.Replace(OrgAddress, @"\/", @"%2C");
            OrgAddress = Regex.Replace(OrgAddress, @"Ростов\s+-\s+на\s+-\s+Дону", "Ростов-на-Дону");
            return LISTORG_URL + OrgAddress;
        }

        /// <summary>
        /// получаем ИНН
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="orgAdress"></param>
        /// <returns></returns>
        public static string GetINNofOrg(string orgName,string orgAdress)
        {
            string path =GetСorrectAdress(orgAdress);
            WebRequest wr = WebRequest.Create(path);
            Stream t=null;
            try
            {
                WebResponse responce = (HttpWebResponse)wr.GetResponse();
                 t = responce.GetResponseStream();
            }
            catch (WebException webex)
            {
                Console.WriteLine(webex.Message);
            }

            try
            {
                var temp = new StreamReader(t);

                var mathes = Regex.Matches(temp.ReadToEnd(), @"<p>([\s\S]+?)<\/p>");
                foreach (Match x in mathes)
                {
                    if (Regex.IsMatch(x.Value, orgName))
                    {
                        return Regex.Match(x.Value, @"\d{10}").Value;
                    }

                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine(ioex.Message);
            }

            return null;
        }

        /// <summary>
        /// метод чисто для проверки ,потом его удалить 
        /// </summary>
        /// <param name="lst"></param>
        static void PrintCompanyInfo(List<Company> lst)
        {
            foreach (var temp in lst)
            {
                Console.WriteLine(temp.name);
                foreach (var tmp in temp.adress)
                    Console.WriteLine(tmp);
                foreach (var tmp in temp.emails)
                    Console.WriteLine(tmp);
                Console.WriteLine("//////////");
            }
        }

     
        static void Main(string[] args)
        {
            
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var t = Companies.GetCompanies();
            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
            PrintCompanyInfo(t);
          
          /*
            WebRequest webr = WebRequest.Create(ParserINN.RND_COMPS_URL);
            WebResponse webresp = webr.GetResponse();
            var t = webresp.GetResponseStream();
            var temp = new StreamReader(t);
           // Console.WriteLine(temp.ReadToEnd());
            var tempp = Parser.adress.Matches(temp.ReadToEnd());
            foreach (Match s in tempp)
                Console.WriteLine(s.ToString());
            */
        }
    }
}
