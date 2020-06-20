using System;

namespace SocNetParser
{
    class MainProgram
    {

        public static VKParser vkApi = new VKParser();
        static void Main()
        {
           //загрузку сделал по одной тематике за раз из-за слабого железа 
          var t = new BusinessPage("bus_companies", "../../../bus_companies.json");

          t.AddCompanyInfo();

              var linqer = new Linqer(t);
           

            linqer.StartLinq();

           
            
            Console.WriteLine("finished");
        }
    }
}