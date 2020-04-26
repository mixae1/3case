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

        /// <summary>
        /// основной конструктор
        /// </summary>
        /// <param name="nm">имя организации </param>
        /// <param name="adrs">адреса оргн-ии </param>
        /// <param name="hs">элект-ые почты </param>
        /// <param name="web">сайт орг-ции</param>
        /// <param name="phn"> теле-ны огр-ии</param>
        public Company(string nm, HashSet<string> adrs, HashSet<string> hs, string web, List<string> phn)
        {
            adress = adrs;
            emails = hs;
            name = nm;
            website = web;
            phones = phn;
        }

        public Company(string name, HashSet<string> adress, HashSet<string> hs)
        {
            this.adress = adress;
            emails = hs;
            this.name = name;
        }
    }
}
