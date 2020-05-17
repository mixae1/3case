﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using SocNetParser.Properties;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;

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
                        Thread.Sleep(60 - (int)secondsPerMinPassed); // контролируем ограничение в 30 запрсов/мин 
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
        private Dictionary<string, int> GetCompsId(string jsonFilePath)
        { 
         return JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(jsonFilePath));
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
                    
                    temp.registraionDomain = parsers.techdate[counter].Item1;
                    temp.UptDomain = parsers.techdate[counter].Item2;
                    temp.expireDomain = parsers.techdate[counter].Item3;

                    if (parsers.Ids.ContainsKey(temp.name))
                        temp.id = parsers.Ids[temp.name];
                    else
                    {
                        temp.id = parsers.Ids.Count + 1;
                        parsers.Ids.Add(temp.name, temp.id);
                    }

                    counter++;
                    Console.WriteLine($"{counter} from 25");
                }
                    
                
            }

            //хрен его знает куда это запихнуть ,если производительность сильно не упадет ,то оставить здесь
            File.WriteAllText(jsonIdPath,JsonSerializer.Serialize(parsers.Ids));
        }





        //выводим инфу о компаниях бизнес-типа
        public void PrintBuisnessPage(BusinessPage page)
        {
            Companies.PrintCompanyInfo(page.comps);
        }


    }
}
