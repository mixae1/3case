using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocNetParser
{
  public  class Category
    {

        public int IdOrg { get; set; }
        public int IdCat { get; set; }

        public virtual CategoryName IdCatNavigation { get; set; }
        public virtual Organization IdOrgNavigation { get; set; }
    }
}
