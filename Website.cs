using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocNetParser
{
    public partial class Website
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? Organization { get; set; }
        public string Domain { get; set; }
        public DateTime? Registred { get; set; }
        public DateTime? Prolongated { get; set; }
        public string ServerCountry { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string LatestMd5 { get; set; }

        public virtual Organization OrganizationNavigation { get; set; }
    }
}
