using System;
using System.Collections.Generic;
using System.Text;

namespace SocNetParser
{  
    /// <summary>
    /// класс для вытяжки айди по каждой таблице 
    /// </summary>
    class IdTables
    {
        public int Org_Id { get; set; }
        public int Web_Id { get; set; }

        public int Add_Id { get; set; }

        public int Inst_Id { get; set; }

        public int Vk_Id { get; set; }
        public IdTables(int org, int web, int add, int inst, int vk)
        {
            Org_Id = org;
            Web_Id = web;
            Add_Id = add;
            Inst_Id = inst;
            Vk_Id = vk;

        }


        public override string ToString()
        {
            return "ord_id" + Org_Id + '\n'
                + "web_id" + Web_Id + '\n'
                + "add_id" + Add_Id + '\n'
                + "inst_id" + Inst_Id + '\n'
                + "vk_id" + Vk_Id;
        }
    }
}
