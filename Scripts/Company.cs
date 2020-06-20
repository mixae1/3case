using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;
namespace SocNetParser
{
    /// <summary>
    /// класс организации первоначальный 
    /// </summary>

    class Company
    {  

            // id для бдшки
        public int id { get; set; }

        public IdTables ids { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }
         
        [JsonPropertyName("adress")]

        public string adressFromJson { get; set; }
        //список физических или юридических адресов
        [JsonIgnore]
        public IList<string> adress { get; set; }


        public int? InstAud { get; set; }

        [JsonIgnore]
        public HashSet<string> emails { get; set; }

        [JsonPropertyName("url")]
        //ссылка на сайт компании
        public List<string> webs { get; set; }
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


        [JsonPropertyName("phone")]
        public List<string> phones { get; set; }

        public DateTime? lastVkPost { get; set; }

        [JsonPropertyName("social")]
        public List<string> socials { get; set; }

        private string vk;

        
        public string Vk { get { return vk?.Substring(vk.IndexOf("vk.com")+7); } set { vk = value; } }

        private string Inst;
        
        public string inst 
        {
            get 
            { return string.IsNullOrEmpty(Inst) ? Inst :  Inst[Inst.Length-1]=='/' ? Inst:Inst+'/' ; }
            set
            {
                Inst = value==null ? null : value.Contains("http") ? value : "https://" +value ;
            }
        }

        public string face { get; set; } = "";

      public TechSiteInfo techinfo { get; set; }
      

        //нужно для сериализации данных с json файла 
        public Company()
        {
           
            inst = "";
            face = "";
            phones = new List<string>();
            emails = new HashSet<string>();
            adress = new List<string>();
            socials = new List<string>();
        }
        
    }
}
