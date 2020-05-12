using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocNetParser
{
    class Linqer
    {
        List<BusinessPage> pages { get;  }

        public Linqer(params  BusinessPage[] page)
        {
            pages = new List<BusinessPage>(page);
            
        }


        public  void StartLinq()
        {
            foreach (var tmp in pages)
            {
                var vkurls = tmp.GetVkUrls();
              
               Task<List<DateTime?>> vktask =Task.Run(()=> GetVkPostDateTime(vkurls));
                /*
                 * 
                 * + еще парсеры: 
                 * 1)сайта для получения инфы о дате регистрации домена и дате окончания лицензии 
                 * 2) инсты
                 * 3) еще какой-нибудь херни 
                 * !!!!!! если будут траблы с производительностью на компе или серваке сразу написать мне!!!!!
                 * 
                */
              Task.WaitAll(vktask);
                var vkres = vktask.Result;
                AddInfoInPage(vkres);
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
            int counter = 0;
            foreach (var tmp in urls)
            {
                DateTime? dt = null;
                counter++;
                if (!string.IsNullOrEmpty(tmp))
                {
                    var vk = new VKGroup(tmp);
                    vk.LoadPosts(2);
                        dt = vk.DTofLastTimePostedWithWall();
                }
                vkUrls.Add(dt);
                Console.WriteLine($"{counter} from 25");
               
            }

            return vkUrls;
        }

        /// <summary>
        /// добавляет полученную инфу в компании определенного бизнес-типа
        /// </summary>
        /// <param name="vkdates"></param>
        /// <param name="techdate"></param>
        void AddInfoInPage(List<DateTime?> vkdates,List<(DateTime,DateTime)> techdate=null)
        {
            Console.WriteLine("adding all info:");
            Console.WriteLine(vkdates.Count+" размер списка ");
            foreach (var tmp in pages.Select(x => x.comps))
            {
                int vkind = 0;
                int techind = 0;
                int counter = 0;
                foreach (var temp in tmp)
                {
                    counter++;
                    temp.lastVkPost = vkdates[vkind];
                    vkind++;
                    if (techdate == null) continue;
                    else
                    { //убрать блок if после добавления парсера сайта 
                        temp.registraionDomain = techdate[techind].Item1;
                        temp.registraionDomain = techdate[techind].Item2;

                    }
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
