using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using SocNetParser.Properties;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SocNetParser
{
    class Linqer
    {
        List<BusinessPage> pages { get;  }
        //пусть здесь будет ,нужно чтоб не указывать полный пусть ,тк в ресурсах нужно полный путь указывать
        private string jsonIdPath = "../../../id.json";

        
      
        public Linqer(params  BusinessPage[] page)
        {
            pages = new List<BusinessPage>(page);
          
        }

        /// <summary>
        /// начинает процесс сбоки информации : обходит вк 
        /// TODO: обойти сайт с технической инфой и инсту прикрутить
        /// </summary>
        public  void StartLinq()
        {
            foreach (var tmp in pages)
            {
                var vkurls = tmp.GetVkUrls();
                var domains = tmp.GetDomains();

               Task<List<DateTime?>> vktask =Task.Run(()=> GetVkPostDateTime(vkurls));
                Task<List<(DateTime? regdate, DateTime? expdate, DateTime? upddate)>> techtask = Task.Run(() => GetTechSiteInfo(domains));
                Task<Dictionary<string, int>> idTask = Task.Run(() => GetCompsId(jsonIdPath));
                
                /*
                 * 
                 * + еще парсеры: 
                 * 2) инсты
                 * 3) еще какой-нибудь херни 
                 * !!!!!! если будут траблы с производительностью на компе или серваке сразу написать мне!!!!!
                 * 
                */
              Task.WaitAll(vktask,techtask);
                var vkres = vktask.Result;
                var techres = techtask.Result;
                var idres = idTask.Result;
                AddInfoInPage(new ParsersInfo(vkres,techres,idres));
            }
        }

        /// <summary>
        /// получаем дату последнего поста в вк
        /// TODO:доделать получение постов и другую херню 
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
       List<DateTime?> GetVkPostDateTime(List<string> urls)
        {
            Console.WriteLine("getting vk info:");
            List<DateTime?> vkUrls = new List<DateTime?>();
            foreach (var tmp in urls)
            {
                DateTime? dt = null;
             
                if (!string.IsNullOrEmpty(tmp))
                {
                    var vk = new VKGroup(tmp);
                    vk.LoadPosts(2);
                        dt = vk.DTofLastTimePostedWithWall();
                }
                vkUrls.Add(dt);
            }

            return vkUrls;
        }

        /// <summary>
        /// основной метод получения инфы с whois 
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        List<(DateTime? regdate, DateTime? expdate, DateTime? upddate)> GetTechSiteInfo(List<string> urls)
        {
            List<(DateTime?, DateTime?, DateTime?)> lst = new List<(DateTime?, DateTime?, DateTime?)>();

            Console.WriteLine("starting parsing Whois.ru");
            WebClient wb = new WebClient();
            int counter = 0; // по условию юзанья там стоит ограничение в 30 запросов за минуту 
            DateTime counterTime = DateTime.Now;
            foreach (var tmp in urls)
            {
                double secondsPerMinPassed = DateTime.Now.Subtract(counterTime).TotalSeconds;
                if (secondsPerMinPassed < 60)
                {
                    if (counter >= 29) //перестраховка
                        Thread.Sleep((60 - (int)secondsPerMinPassed)*1000); // контролируем ограничение в 30 запрсов/мин 
                    counter = 0;                            // если уже прошла минута и было сделано 30 то засыпаем на остаток минуты
                }
                else
                    counterTime = DateTime.Now; // если уже прошла минута ,то просто переприсваиваем счетчик времени

                if (string.IsNullOrEmpty(tmp))
                {
                    lst.Add((null, null, null));
                    continue;
                }

                string ans = null;
                try
                {
                    ans = wb.DownloadString(Resources.TechSiteApi+tmp); 
                }catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+" error was at "+tmp);
                }

                if (ans == null) { lst.Add((null, null, null)); continue; }
                var temp = GetDates(ans,tmp);
                counter++;

                DateTime? nll = null;
                DateTime? crDate = string.IsNullOrEmpty(temp.creationDate) ? nll: DateTime.Parse(temp.creationDate.Trim());
                DateTime? update = string.IsNullOrEmpty(temp.updDate)?  nll: DateTime.Parse(temp.updDate.Trim()); 
                DateTime ? expDate= string.IsNullOrEmpty(temp.expDate) ? nll: DateTime.Parse(temp.expDate.Trim());
                lst.Add((crDate, update, expDate));

               
            }

            return lst;
        }

        //вспомогательный метод для получения инфы с апишки whois
        private (string creationDate, string updDate, string expDate) GetDates(string ans,string domain)
        {
            if (SiteParser.CheckAvail.IsMatch(ans))
            {
                if (Regex.IsMatch(domain, @"\.ru"))
                    return (
                          SiteParser.RusDomainCr.Match(ans).Groups[1].Value, null,
                          SiteParser.RusDomianTill.Match(ans).Groups[1].Value);
                return (
                     SiteParser.EngDomainCr.Match(ans).Groups[1].Value,
                     SiteParser.EngDomainUpd.Match(ans).Groups[1].Value,
                     SiteParser.EngDomainExpr.Match(ans).Groups[1].Value);     
               
            }
            return (null, null, null);
        }

        // получаем инфу об айдишках оргов
        public Dictionary<string, int> GetCompsId(string jsonFilePath)
        { 
         return JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(jsonFilePath));
        }

        /// <summary>
        /// получаем 
        /// </summary>
        /// <returns></returns>
        protected List<int?> GetInstAccAud()
        {
              var tmp=  Regex.Replace(File.ReadAllText("InstData.json"), @",]", "]"); // костыль тк в файле баг и его нормально не десериализовать; TODO: исправить баг в коде на пыхе
                var temps = JsonSerializer.Deserialize<string[]>(tmp);
                int? t = null;
                return temps.Select(x => x == null ? t : int.Parse(Regex.Replace(Regex.Replace(x, "k", "00"), @"[\.|,]", ""))).ToList();
              
                                         
        }

        // метод запускающий парсер инсты
        public List<int?> UploadInstFile()
        {   
           // File.Create("InstData.json");
         
            //new3t.php -доработанный файл Марины ,  оригнальный на гите лежит с названием new3.php 
            ProcessStartInfo info = new ProcessStartInfo("php.exe", "../../../new3t.php");
            info.UseShellExecute = false;
            info.ErrorDialog = false;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;


            Process p = new Process();
            p.StartInfo = info;

          p.Start();
            p.WaitForExit();
            return GetInstAccAud();
            
        }



        /// <summary>
        /// добавляет полученную инфу в компании определенного бизнес-типа
        /// </summary>
        /// <param name="vkdates"></param>
        /// <param name="techdate"></param>
        private  void AddInfoInPage(ParsersInfo parsers)
        {
            Console.WriteLine("adding all info:");
            Console.WriteLine(parsers.techdate.Count+" размер списка " ); //для проверки потом удалить
            foreach (var tmp in pages.Select(x => x.comps))
            {
               
                int counter = 0;
                foreach (var temp in tmp)
                {
                    
                    temp.lastVkPost =parsers.vkdates[counter];
                    
                    temp.techinfo =new TechSiteInfo(parsers.techdate[counter].Item1,
                    parsers.techdate[counter].Item2,
                     parsers.techdate[counter].Item3);

                    if (parsers.Ids.ContainsKey(temp.name))
                        temp.id = parsers.Ids[temp.name];
                    else
                    {
                        temp.id = parsers.Ids.Count + 1;
                        parsers.Ids.Add(temp.name, temp.id);
                    }

                    counter++;
                    Console.WriteLine($"{counter} from  {parsers.techdate.Count}");
                }
                    
                
            }

            //хрен его знает куда это запихнуть ,если производительность сильно не упадет ,то оставить здесь
            File.WriteAllText(jsonIdPath,JsonSerializer.Serialize(parsers.Ids));
            

        }

        public List<Organization> PrepareToDB()
        {
            var lst = new List<Organization>();
            foreach (var tmp in pages.Select(x=>x.comps))
            {
                foreach (var temp in tmp)
                {
                    var vksocial = new SocialAccount()
                    { Organization = temp.id, Link = temp.Vk,type = social_type.vk , LastUpdate=temp.lastVkPost };

                    var instsocial = new SocialAccount()
                    { Organization = temp.id, Link = temp.inst,type =social_type.instagram , Auditory=temp.InstAud };

                    var adr = new Address()
                    {
                        Email = temp.emails.FirstOrDefault(),
                        Organization = temp.id,
                        Phone = temp.phones.FirstOrDefault(),
                        City = "Ростов-на-Дону",
                        adress = temp.adress.FirstOrDefault()
                    };

                    var web = new Website()
                    {
                        Organization = temp.id,
                        Domain = temp.Website,
                        Registred = temp.techinfo.registraionDomain,
                        LastUpdate =null,
                        Prolongated = temp.techinfo.expireDomain
                    };

                    lst.Add(new Organization(temp.name,new Website[] { web },new Address[] { adr }, new HashSet<SocialAccount>(new SocialAccount[] { vksocial, instsocial })));
                }
            }
            return lst;
        }


        public void LoadToDB()
        {
            var comps = PrepareToDB();
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<social_type>("social_type");
            using (DBContext db = new DBContext())
            {
               
                foreach (var tmp in comps)
                    db.Add(tmp);
                db.SaveChanges();
            }
        }




        //выводим инфу о компаниях бизнес-типа
        public void PrintBuisnessPage(BusinessPage page)
        {
            Companies.PrintCompanyInfo(page.comps);
        }


    }
}
