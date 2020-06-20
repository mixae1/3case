using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocNetParser
{
   public class CategoryName
    {

        public CategoryName()
        {
            Category = new HashSet<Category>();
        }

       

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Category> Category { get; set; }
    }
}
