using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocNetParser
{ 


        public  enum social_type
        {
            facebook, instagram, vk,ok

        }


    public partial class SocialAccount
    {
      


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? Organization { get; set; }
        public int? Auditory { get; set; }
        public string Link { get; set; }
        public DateTime? LastUpdate { get; set; }

        [Column(TypeName = "social_type")]
        public social_type type { get; set; }
     
        public virtual Organization OrganizationNavigation { get; set; }


        public override string ToString()
        {
            return '\n' + type + '\n' +
                 "id " + Organization +
                 "auditory " + Auditory + '\n' +
               "link" + Link + '\n' +
                "lastupdate " + LastUpdate;
        }

    }
}
