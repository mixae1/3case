using System;
using System.Collections.Generic;
using System.Text;

namespace SocNetParser
{

    /// <summary>
    /// страница для каждого бизнеса с сайта , нужно для многопоточности и асинхроности 
    /// </summary>

    class BusinessPage
    {

        public string webName { get; }
        public int totalPages { get; set; }
        public List<Company> comps { get; set; }

        public BusinessPage(string webname)
        {
            webName = webname;
        }

    }
}
