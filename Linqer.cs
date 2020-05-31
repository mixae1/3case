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
using VkNet.Model.Attachments;
using System.ComponentModel;

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
                var names = tmp.GetCompNames();
                File.WriteAllText("InstUrls.json", JsonSerializer.Serialize(tmp.GetInsTurls()));
                //File.WriteAllText("InstData.json", "");

               Task<List<DateTime?>> vktask =Task.Run(()=> GetVkPostDateTime(vkurls));
                Task<List<(DateTime? regdate, DateTime? expdate, DateTime? upddate)>> techtask = Task.Run(() => GetTechSiteInfo(domains));
                Task<Dictionary<string,IdTables>> idTask = Task.Run(() => GetCompsId(names));
                Task<List<int?>> instTask = Task.Run(() => GetInstAccAud());
                
             
              Task.WaitAll(vktask,techtask,instTask);
                var vkres = vktask.Result;
                var techres = techtask.Result;
                var idres = idTask.Result;
                var instres = instTask.Result;

                AddInfoInPage(new ParsersInfo(vkres,techres,idres,instres));
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
            //File.WriteAllText("techsite.json", JsonSerializer.Serialize(lst));
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
        // делаем асинхронно для увеличения производительности
        // мб потом вытянем все таблицы сразу а не по запросу
        public Dictionary<string,IdTables> GetCompsId(HashSet<string> comps)
        {
            Dictionary<string,IdTables> dct = new Dictionary<string,IdTables>();
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<social_type>("social_type");
            DBContext db = new DBContext();
            var org_temps = db.Organization.ToList();
          
                foreach (Organization ord in org_temps)
                {
                    if (ord.Name == null) continue;

                if (comps.Contains(ord.Name) && !dct.ContainsKey(ord.Name))
                    dct.Add(ord.Name, new IdTables(ord.Id,
                        db.Website.Where(x => x.Organization == ord.Id).FirstOrDefault().Id,
                        db.Address.Where(x => x.Organization == ord.Id).FirstOrDefault().Id,
                        db.SocialAccount.Where(x => x.Organization == ord.Id).Where(x => x.type == social_type.instagram).First().Id,
                        db.SocialAccount.Where(x => x.Organization == ord.Id).Where(x => x.type == social_type.vk).First().Id));

                }

        

            return dct;
        }

       // вспомогательный метод ,который из джсон файла инсты получает данные
       // пока не разберемся с инстой будем заранее полученные данные из инсты брать

       public List<int?> GetInstAccAud()
        {
              var tmp=  Regex.Replace(File.ReadAllText("InstData.json"), @",]", "]"); // костыль тк в файле баг и его нормально не десериализовать; TODO: исправить баг в коде на пыхе
                var temps = JsonSerializer.Deserialize<string[]>(tmp);
                int? t = null;
                return temps.Select(x => x == null ? t : int.Parse(Regex.Replace(Regex.Replace(x, "k", "00"), @"[\.|,]", ""))).ToList();
              
                                         
        }

        /// <summary>
        /// метод для запуска парсера инсты на пыхе 
        /// </summary>
        /// <returns></returns>
        public List<int?> UploadInstFile()
        { 
            
            // File.Create("InstData.json");
            Console.WriteLine("starting inst parser:");
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
            var bp = pages.Select(x => x.Name).ToList();
            foreach (var tmp in pages.Select(x => x.comps))
            {
                int curnum = 0;
                int counter = 0;
                foreach (var temp in tmp)
                {
                    
                    temp.lastVkPost =parsers.vkdates[counter];

                    temp.InstAud = parsers.instSubsNum[counter];
                    temp.techinfo =new TechSiteInfo(parsers.techdate[counter].Item1,
                    parsers.techdate[counter].Item2,
                     parsers.techdate[counter].Item3);

                    if (parsers.Ids.ContainsKey(temp.name))
                    {
                        temp.ids = parsers.Ids[temp.name];
                    }
                   

                    counter++;
                    Console.WriteLine($"{counter} from  {parsers.techdate.Count}");
                }
                Console.WriteLine("//////////////");
               LoadToDB(bp[curnum]);
                curnum++;
                
            }


        }

        //TODO : РЕАЛИЗОВАТЬ СЛОВАРЬ : КЛЮЧ АЙДИ КАТЕГОРИИ: ЗНАЧ МНОЖЕСТВО КАТЕГРИИИМЯ
        public List<Organization> PrepareToDB(string cur)
        {
            var lst = new List<Organization>();
            int counter = 1;




            foreach (var tmp in pages.Select(x=>x.comps))
            {
                foreach (var temp in tmp)
                {
                    var vksocial = new SocialAccount()
                    { Id=temp.ids.Vk_Id,Organization = temp.id, Link = temp.Vk,type = social_type.vk , LastUpdate=temp.lastVkPost };

                    var instsocial = new SocialAccount()
                    { Id=temp.ids.Inst_Id, Organization = temp.id, Link = temp.inst,type =social_type.instagram , Auditory=temp.InstAud };

                    var adr = new Address()
                    {
                        Id = temp.ids.Add_Id,
                        Email = temp.emails.FirstOrDefault(),
                        Organization = temp.id,
                        Phone = temp.phones.FirstOrDefault(),
                        City = "Ростов-на-Дону",
                        adress = temp.adress.FirstOrDefault()
                    };

                    var web = new Website()
                    {   
                        Id=temp.ids.Web_Id,
                        Organization = temp.id,
                        Domain = temp.Website,
                        Registred = temp.techinfo.registraionDomain,
                        LastUpdate =null,
                        Prolongated = temp.techinfo.expireDomain
                       
                    };



                    var catname = new CategoryName()
                    {
                        CategoryId = counter,
                        categoryName = cur
                    };

                    var cat = new Category()
                    {
                        Organization = temp.id,
                        id_cat = counter,
                        Categories = new CategoryName[] { catname }
                    };

                    counter++;
                    lst.Add(new Organization(temp.id,temp.name,new Website[] { web },new Address[] { adr }, new HashSet<SocialAccount>(new SocialAccount[] { vksocial, instsocial }),new Category[] {cat }));
                }
            }
            return lst;
        }


        public void LoadToDB(string name)
        {
            var comps = PrepareToDB(name);
            DBContext db = new DBContext();
            foreach (var x in comps)
            {
                
                var t = db.Organization.Find(x.Id);
                if (t != null)
                {
                    t.SocialAccount = x.SocialAccount;
                    foreach (var temp in t.SocialAccount)
                    {   
                       
                        temp.OrganizationNavigation = t;
                    }
                    
                    t.Website = x.Website;
                        t.Website.First().OrganizationNavigation = t;
/*
                    t.Address = x.Address;
                    foreach (var temp in t.Address)
                        temp.OrganizationNavigation = t;
                   /* 
                     t.Categories = x.Categories;
                     foreach (var temp in t.Categories)
                     {   

                         temp.OrganizationNavigation = t;
                     }  
                     /*
                    db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    foreach(var tmp in t.Address)
                    db.Entry(tmp).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    foreach (var tmp in t.Website)
                      db.Entry(tmp).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    foreach (var tmp in t.SocialAccount)
                        db.Entry(tmp).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        */
                }

            
              //  UpdateOrganization(x);

            }
            db.SaveChanges();
            
            
        }


        public void UpdateOrganization(Organization org)
        {
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<social_type>("social_type");
            DBContext db = new DBContext();
            var entity = db.Organization.Attach(org);
            db.Entry(org).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();

        }

    


        //выводим инфу о компаниях бизнес-типа
        public void PrintBuisnessPage(BusinessPage page)
        {
            Companies.PrintCompanyInfo(page.comps);
        }


    }
}
