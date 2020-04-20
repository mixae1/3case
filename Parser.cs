using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{

    //пусть пока будет интернал ,потом разберемся ,наверное вообще статичным сделаем 
    class Parser
    {   /// <summary>
    /// ссылка для запроса к сайту ,где инн берем
    /// </summary>
        const string LISTORG_URL= "https://www.list-org.com/search?type=address&val=";

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

     
        static void Main(string[] args)
        {
            string path = "Ворошиловский проспект, 91 / 1 Ростов - на - Дону";
            Console.WriteLine(GetINNofOrg("РИС",path));
          
        }
    }
}
