using System;
using System.Collections.Generic;
using System.Text;

namespace SocNetParser
{ 

    /// <summary>
    /// класс с инфой о данных с сайта whois 
    /// </summary>
    class TechSiteInfo
    {  
        // дата регистрации домена
        public DateTime? registraionDomain { get; set; }

        //дата окончания срока регистрации
        public DateTime? expireDomain { get; set; }

        // дата обновления ; у сайтов с .ru ее нет
        public DateTime? UptDomain { get; set; }

        public TechSiteInfo(DateTime? regs, DateTime? exp, DateTime? upt)
        {
            registraionDomain = regs; 
            UptDomain = upt;
            expireDomain = regs;
           
        }


        public override string ToString()
        {
            return "creation domain time " + registraionDomain + "\n"
                + "update domain time" + UptDomain + "\n"
                + "expire domain time" + expireDomain;
        }

    }
}
