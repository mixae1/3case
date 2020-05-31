using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace SocNetParser
{
    /// <summary>
    /// класс 
    /// </summary>

    class Company
    {  
        //АТТРИБУТЫ ЛУЧШЕ ОСТАВИТЬ ,ХРЕН ЕГО ЗНАЕТ КАК ОНИ БУДУТ В ДЖСОН ФАЙЛЕ НАЗВАНЫ ,ЗДЕСЬ ИХ ИЗМЕНИТЬ БЫСТРЕЕ БУДЕТ

            // id для бдшки
        public int id { get; set; }

        public IdTables ids { get; set; }


        public string name { get; set; }
         
        [JsonPropertyName("adress")]
        //список физических или юридических адресов
        public HashSet<string> adress { get; set; }


        public int? InstAud { get; set; }

        [JsonIgnore]
        public HashSet<string> emails { get; set; }

        [JsonPropertyName("web")]
        //ссылка на сайт компании
        public string Website
        {
            get
            {

                return web == null ?
                        null : web.IndexOf("www.") == -1 ? web : web.Substring(web.IndexOf("www.") + 4);
            }
            set
            {
                web = value;
            }
        }

        private string web;


         [JsonIgnore]
        public List<string> phones { get; set; }

        public DateTime? lastVkPost { get; set; }

        private string vk;

        [JsonPropertyName("vk")]
        public string Vk { get { return vk?.Substring(vk.IndexOf('/')+1); } set { vk = value; } }

        private string Inst;
        [JsonPropertyName("inst")]
        public string inst 
        {
            get 
            { return Inst == null ? Inst : "https://www." + Inst+"/"; }
            set
            {
                Inst = value;
            }
        }

        public string face { get; set; } = "";

      public TechSiteInfo techinfo { get; set; }
      

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
