using System;
using System.Collections.Generic;
using System.Text;

namespace SocNetParser
{ 

    /// <summary>
    /// класс для инфы собранной со всех парсеров
    /// </summary>
    class ParsersInfo
    { 
        //список последних постов вк 
       public List<DateTime?> vkdates { get; private set; }
        //список с датами:дата создания домена обновления и окончания юзанья домена
       public List<(DateTime?, DateTime?, DateTime?)> techdate { get; private set; }
        //словарь с айдишками оргов
        public Dictionary<string,IdTables> Ids { get; private set; }

        // список кол-во подписоты инсты
        public List<int?> instSubsNum { get; private set; }
        public ParsersInfo(List<DateTime?> vk, List<(DateTime?, DateTime?, DateTime?)> techs, Dictionary<string,IdTables> ids, List<int?> instLst)
        {
            vkdates = vk;
            techdate = techs;
            Ids = ids;
            instSubsNum = instLst;
        }
        public ParsersInfo(List<DateTime?> vk, List<(DateTime?, DateTime?, DateTime?)> techs, List<int?> instLst)
        {

            vkdates = vk;
            techdate = techs;
            instSubsNum = instLst;
        }

    }
}
