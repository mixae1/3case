using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocNetParser
{
    public partial class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? Organization { get; set; }
        public string City { get; set; }
        public string adress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Coordinates { get; set; }

        public virtual Organization OrganizationNavigation { get; set; }

        public override string ToString()
        {
            return '\n'+ "id " + Organization + '\n'
                + Email + '\n' +
                Phone + '\n' + adress;

        }

    }
}
