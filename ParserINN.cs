using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace SocNetParser
{
    //пусть пока будет интернал ,потом разберемся ,наверное вообще статичным сделаем 
    class ParserINN
    {   /// <summary>
        /// ссылка для запроса к сайту ,где инн берем
        /// </summary>
        const string LISTORG_URL = "https://www.list-org.com/search?type=address&val=";
        /// <summary>
        /// ссылка для запроса к основному сайту (пока ведет к авто тематике)
        /// </summary>



        //подготовка адреса сайта к запросу
        public static string GetСorrectAdress(string OrgAddress)
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
        public static string GetINNofOrg(string orgName, string orgAdress)
        {
            string path = GetСorrectAdress(orgAdress);
            WebRequest wr = WebRequest.Create(path);
            Stream t = null;
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
        public static void PrintCompanyInfo(List<Company> lst)
        {
            foreach (var temp in lst)
            {
                Console.WriteLine(temp.name);
                foreach (var tmp in temp.adress)
                    Console.WriteLine(tmp);
                foreach (var tmp in temp.emails)
                    Console.WriteLine(tmp);
                Console.WriteLine(temp.website);
                Console.WriteLine("//////////");
            }
        }
    }
}
