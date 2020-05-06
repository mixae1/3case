using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
namespace SocNetParser
{
    /// <summary>
    /// класс 
    /// </summary>

    class Company
    {  
        //АТТРИБУТЫ ЛУЧШЕ ОСТАВИТЬ ,ХРЕН ЕГО ЗНАЕТ КАК ОНИ БУДУТ В ДЖСОН ФАЙЛЕ НАЗВАНЫ ,ЗДЕСЬ ИХ ИЗМЕНИТЬ БЫСТРЕЕ БУДЕТ

        public string name { get; set; }
         
        [JsonPropertyName("adress")]
        //список физических или юридических адресов
        public HashSet<string> adress { get; set; }


        [JsonIgnore]
        public HashSet<string> emails { get; set; }

        [JsonPropertyName("web")]
        //ссылка на сайт компании
        public string website { get; set; }

         [JsonIgnore]
        public List<string> phones { get; set; }

        public DateTime? lastVkPost { get; set; }

        private string vk;

        [JsonPropertyName("vk")]
        public string Vk { get { return vk?.Substring(vk.IndexOf('/')+1); } set { vk = value; } }

        public string inst { get; set; } = "";

        public string face { get; set; } = "";


        /// <summary>
        /// основной конструктор
        /// </summary>
        /// <param name="nm">имя организации </param>
        /// <param name="adrs">адреса организации </param>
        /// <param name="hs">электронные почты организации</param>
        /// <param name="web">сайт организации</param>
        /// <param name="phn"> телефоны организации</param>
        public Company(string nm, HashSet<string> adrs, HashSet<string> hs=null, string web=null, List<string> phn=null)
        {
            adress = adrs;
            emails = hs;
            name = nm;
            website = web;
            phones = phn;
        }

        //нужно для сериализации данных с json файла 
        //на случай если парсер яндекса останется на питухоне
       
        public Company()
        {
           
            inst = "";
            face = "";
            phones = new List<string>();
            emails = new HashSet<string>();
            adress = new HashSet<string>();
        }
        
    }
}
