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

        public string website { get; set; }

        public Company(string name, HashSet<string> adress, HashSet<string> hs,string web)
        {
            this.adress = adress;
            emails = hs;
            this.name = name;
            website = web;
        }

        public Company(string name, HashSet<string> adress, HashSet<string> hs)
        {
            this.adress = adress;
            emails = hs;
            this.name = name;
          
        }

    }
}
