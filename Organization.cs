using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design;
namespace SocNetParser
{
    public partial class Organization
    {
        public Organization()
        {
            Address = Address;
            SocialAccount = new HashSet<SocialAccount>();
            Website = Website;
        }


        public Organization(ICollection<Website> web, ICollection<Address> ad, HashSet<SocialAccount> acc)
        {
            Address = ad;
            Website = web;
            SocialAccount = acc;
        }

      
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Opened { get; set; }
        public DateTime? Closed { get; set; }
        public string Inn { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<SocialAccount> SocialAccount { get; set; }
        public virtual ICollection<Website> Website  { get; set; }

        // public Website web { get { return Website.First(); }  }

        public override string ToString()
        {
            string s = '\n'+ "id " + Id + '\n' + Name + '\n' + Address.ToString();
            foreach (var t in SocialAccount)
                s += t.ToString();
            return s + Website.FirstOrDefault();
        }


    }
}
