﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


        public Organization(string name, ICollection<Website> web, ICollection<Address> ad, HashSet<SocialAccount> acc)
        {
            Address = ad;
            Website = web;
            SocialAccount = acc;
            Name = name;
        }

        public Organization(int id,string name, ICollection<Website> web, ICollection<Address> ad, HashSet<SocialAccount> acc, ICollection<Category> cats)
        {
            Address = ad;
            Website = web;
            SocialAccount = acc;
            Name = name;
            Categories = cats;
            Id = id;
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Opened { get; set; }
        public DateTime? Closed { get; set; }
        public string Inn { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<SocialAccount> SocialAccount { get; set; }
        public virtual ICollection<Website> Website  { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        // public Website web { get { return Website.First(); }  }

        public override string ToString()
        {
            string s = '\n' + "id " + Id + '\n' + Name + '\n';/* + Address.FirstOrDefault().ToString();*/
           // foreach (var t in SocialAccount)
           //     s +='\n'+ t.ToString();
            return s; /*+ Website.FirstOrDefault();*/
        }


    }
}
