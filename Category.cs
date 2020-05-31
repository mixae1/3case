using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocNetParser
{
  public  class Category
    {
       
        public int? Organization { get; set; }

       [Key]
        public int id_cat { get; set; }

        public virtual ICollection<CategoryName> Categories { get; set; }
        public virtual Organization OrganizationNavigation { get; set; }

    }
}
