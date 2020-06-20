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
                
               Task<List<DateTime?>> vktask =Task.Run(()=> GetVkPostDateTime(vkurls));
                Task<List<(DateTime? regdate, DateTime? expdate, DateTime? upddate)>> techtask = Task.Run(() => GetTechSiteInfo(domains));
                Task<Dictionary<string,IdTables>> idTask = Task.Run(() => GetCompsId(names));
                Task<List<int?>> instTask = Task.Run(() => UploadInstFile());
                
             
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
            int i = 1;
            foreach (var tmp in urls)
            {
                DateTime? dt = null;
             
                if (!string.IsNullOrEmpty(tmp))
                {
                    var vk = new VKGroup(tmp);
                    vk.LoadPosts(2);
                        dt = vk.DTofLastTimePostedWithWall();
                    Console.WriteLine(i + "/" + 25+" "+tmp);
                }
                vkUrls.Add(dt);
            }
            Console.WriteLine("vk finish ");
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

            Console.WriteLine("get whois finish");
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
        
        private Dictionary<string,IdTables> GetCompsId(HashSet<string> comps)
        {
            Console.WriteLine("get id comps starts");
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


            Console.WriteLine("get id fin starts");
            return dct;
        }

       // вспомогательный метод ,который из джсон файла инсты получает данные
      
      private List<int?> GetInstAccAud()
        {
              var tmp=  Regex.Replace(File.ReadAllText("InstData.json"), @",]", "]"); // костыль тк в файле баг и его нормально не десериализовать; TODO: исправить баг в коде на пыхе
                var temps = JsonSerializer.Deserialize<string[]>(tmp);
            int? t = null;
            var x = temps.Select(x => x == null ? x : x.Replace("Welcome", ""));
        return x.Select(x => string.IsNullOrEmpty(x) ? t : int.Parse(Regex.Replace(Regex.Replace(x, "k", "00"), @"[\.|,]", ""))).ToList();
                                                 
        }

        /// <summary>
        /// метод для запуска парсера инсты на пыхе 
        /// </summary>
        /// <returns></returns>
        public List<int?> UploadInstFile()
        {

            File.WriteAllLines("InstData.json", new string[] { "" });
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
            Console.WriteLine("inst parser finish");
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
            foreach (var tmp in pages)
            {
                int curnum = 0;
                int counter = 0;
                foreach (var temp in tmp.comps)
                {
                    
                    temp.lastVkPost =parsers.vkdates[counter];

                    temp.InstAud = parsers.instSubsNum[counter];
                    temp.techinfo =new TechSiteInfo(parsers.techdate[counter].Item1,
                    parsers.techdate[counter].Item2,
                     parsers.techdate[counter].Item3);

                    if (parsers.Ids.ContainsKey(temp.name))
                    {
                        temp.ids = parsers.Ids[temp.name];
                        temp.id = temp.ids.Org_Id;
                    }
                   

                    counter++;
                    Console.WriteLine($"{counter} from  {parsers.techdate.Count}");
                }
                Console.WriteLine("//////////////");
               LoadToDB(bp[curnum]);
                curnum++;
                
            }


        }

      private List<Organization> PrepareToDB()
        {
            var lst = new List<Organization>();
            int counter = 1;

            foreach (var tmp in pages.Select(x=>x.comps))
            {
               
                
                foreach (var temp in tmp)
                {

                    var vksocial = new SocialAccount()
                    { Id=temp.ids==null ? 0: temp.ids.Vk_Id,Organization = temp.id, Link = temp.Vk,type = social_type.vk , LastUpdate=temp.lastVkPost };

                    var instsocial = new SocialAccount()
                    { Id= temp.ids == null ? 0 : temp.ids.Inst_Id, Organization = temp.id, Link = temp.inst,type =social_type.instagram , Auditory=temp.InstAud };

                    var adr = new Address()
                    {
                        Id = temp.ids == null ? 0 : temp.ids.Add_Id ,
                        Email = temp.emails.FirstOrDefault(),
                        Organization = temp.id,
                        Phone = temp.phones.FirstOrDefault(),
                        City = "Ростов-на-Дону",
                        adress = temp.adress.FirstOrDefault()
                    };

                    var web = new Website()
                    {   
                        Id= temp.ids == null ? 0: temp.ids.Web_Id,
                        Organization = temp.id,
                        Domain = temp.Website,
                        Registred = temp.techinfo.registraionDomain,
                        LastUpdate =null,
                        Prolongated = temp.techinfo.expireDomain
                       
                    };




                 

                    counter++;
                    lst.Add(new Organization(temp.id,temp.name,new Website[] { web },new Address[] { adr }, new HashSet<SocialAccount>(new SocialAccount[] { vksocial, instsocial })));
                }
            }
            return lst;
        }


        public void LoadToDB(string name)
        {
            var comps = PrepareToDB();
            DBContext db = new DBContext();
            int lastindexofCategsName = db.CategoryName.ToList().Max(x=>x.Id);

            //для добавления оргов которых еще нет в бд 
            CategoryName catname = new CategoryName()
            { Name = name, Id = lastindexofCategsName + 1 };

            

           int id = 1;
            foreach (var x in comps)
            {  

                var t = db.Organization.Find(x.Id);
               
                if (t != null)
                {
                    Console.WriteLine($"{t.Name}  was founded!");
                    t.SocialAccount = x.SocialAccount;
                    foreach (var temp in t.SocialAccount)
                    {

                        temp.OrganizationNavigation = t;
                    }

                    t.Website = x.Website;
                    t.Website.First().OrganizationNavigation = t;

                    t.Address = x.Address;
                    t.Address.First().OrganizationNavigation = t;

                    t.Category = x.Category;
                    t.CategoryNavigation = x.CategoryNavigation;
                  
                  
                }
                else
                {

                    Category cat = new Category
                    {
                        IdCatNavigation = catname,
                        IdCat = id,
                        IdOrg = x.Id,
                        IdOrgNavigation = x
                    };

                    db.Category.Add(cat);
                    x.Category = cat.IdCat;
                    x.CategoryNavigation.Add(cat);
                    id++;

                    db.Organization.Add(x);
                }
            

            }
            db.SaveChanges();
            
            
        }

    }
}
