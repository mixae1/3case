using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocNetParser
{
   public class CategoryName
    {   
        public string categoryName { get; set; }

        [Key]
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
