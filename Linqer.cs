using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

namespace SocNetParser
{
    class Linqer
    {
        List<BusinessPage> pages { get; set; }

        public Linqer(params  BusinessPage[] page)
        {
            pages = new List<BusinessPage>(page);
            
        }


        public void StartLinq()
        {
            foreach (var tmp in pages)
            {
              
            }
        }


        DateTime GetLastVkPost(string domain)
        {
          var t= new VKGroup(domain);
            t.LoadPosts(2);
            return t.DTofLastTimePostedWithWall();
        }

    }
}
