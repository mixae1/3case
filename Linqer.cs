using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using SocNetParser.Properties;
using System.Text.RegularExpressions;

namespace SocNetParser
{
    class Linqer
    {
        List<BusinessPage> pages { get;  }

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
                AddInfoInPage(vkres,techres);
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
           // int counter = 0;
            foreach (var tmp in urls)
            {
                DateTime? dt = null;
               // counter++;
                if (!string.IsNullOrEmpty(tmp))
                {
                    var vk = new VKGroup(tmp);
                    vk.LoadPosts(2);
                        dt = vk.DTofLastTimePostedWithWall();
                }
                vkUrls.Add(dt);
                //Console.WriteLine($"{counter} from 25");
               
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
                if (secondsPerMinPassed < 60 && counter == 30)
                {
                    Thread.Sleep(60-(int)secondsPerMinPassed); // контролируем ограничение в 30 запрсов/мин 
                    counter = 30;                            // если уже прошла минута и было сделано 30 то засыпаем на остаток минуты
                    counterTime = DateTime.Now;
                }


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

        //вспомогательный метод
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




        /// <summary>
        /// добавляет полученную инфу в компании определенного бизнес-типа
        /// </summary>
        /// <param name="vkdates"></param>
        /// <param name="techdate"></param>
        void AddInfoInPage(List<DateTime?> vkdates,List<(DateTime?,DateTime?, DateTime?)> techdate=null)
        {
            Console.WriteLine("adding all info:");
            Console.WriteLine(techdate.Count+" размер списка " );
            foreach (var tmp in pages.Select(x => x.comps))
            {
               
                int counter = 0;
                foreach (var temp in tmp)
                {
                    
                    temp.lastVkPost = vkdates[counter];
                    
                    temp.registraionDomain = techdate[counter].Item1;
                    temp.UptDomain = techdate[counter].Item2;
                    temp.expireDomain = techdate[counter].Item3;

                    counter++;
                    Console.WriteLine($"{counter} from 25");
                }
                    
                
            }
        }

        public void PrintBuisnessPage(BusinessPage page)
        {
            Companies.PrintCompanyInfo(page.comps);
        }


    }
}
