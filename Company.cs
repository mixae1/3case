using System.Collections.Generic;

namespace SocNetParser
{
    /// <summary>
    /// класс 
    /// </summary>

    class Company
    {
        public string name { get; set; }
        //  string inn { get; set; }

        //список физических или юридических адресов
        public HashSet<string> adress { get; set; }

        public HashSet<string> emails { get; set; }
        //ссылка на сайт компании
        public string website { get; set; }

        public List<string> phones { get; set; }

        public string vk { get; set; }
        public string face { get; set; }
        
        public string inst { get; set; }


        /// <summary>
        /// основной конструктор
        /// </summary>
        /// <param name="nm">имя организации </param>
        /// <param name="adrs">адреса организации </param>
        /// <param name="hs">электронные почты организации</param>
        /// <param name="web">сайт организации</param>
        /// <param name="phn"> телефоны организации</param>
        public Company(string nm, HashSet<string> adrs, HashSet<string> hs, string web, List<string> phn,string VK,string Face,string Inst)
        {
            adress = adrs;
            emails = hs;
            name = nm;
            website = web;
            phones = phn;
            vk = VK;
            face = Face;

        }

        public Company(string name, HashSet<string> adress, HashSet<string> hs)
        {
            this.adress = adress;
            emails = hs;
            this.name = name;
        }
    }
}
