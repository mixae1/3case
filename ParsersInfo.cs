using System;
using System.Collections.Generic;
using System.Text;

namespace SocNetParser
{
    class ParsersInfo
    { 
        //список последних постов вк 
        //МБ придется создать класс для данных с вк и уже вместо датетайм этот класс запихнуть
       public List<DateTime?> vkdates { get; private set; }
        //список с датами:дата создания домена обновления и окончания юзанья домена
       public List<(DateTime?, DateTime?, DateTime?)> techdate { get; private set; }
        //словарь с айдишками оргов
        public Dictionary<string, int> Ids { get; private set; }

        public ParsersInfo(List<DateTime?> vk, List<(DateTime?, DateTime?, DateTime?)> techs, Dictionary<string, int> ids)
        {
            vkdates = vk;
            techdate = techs;
            Ids = ids;
        }
      
        public void VkInfo()
        {
            foreach (var x in vkdates)
                Console.WriteLine("last post date " + x);
        }

        /// <summary>
        /// метод для вывода инфы с сайта whois
        /// </summary>
        public void TechInfo()
        {
            foreach (var x in techdate)
                Console.WriteLine("created:" + x.Item1 + " updated " + x.Item2 + " expired: " + x.Item3);
        }

        /// <summary>
        /// метод для вывода id оргов
        /// </summary>
        public void IdInfo()
        {
            foreach (var x in Ids)
                Console.WriteLine("company:" + x.Key+" id:"+x.Value);
        }

    }
}
