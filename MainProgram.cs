using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SocNetParser
{

    partial class MainProgram
    {

        static async void GetData()
        {
            HttpClient h = new HttpClient();
            HttpResponseMessage t = await h.GetAsync("https://atlant-don.ru/");
            string inp = await t.Content.ReadAsStringAsync();
            // var temp=Companies.websites.Matches(inp);
            //   foreach(var x in  temp)
            Console.WriteLine(inp);
        }



        public static bool AppAdress(string adress, string[] streets)
        {
            foreach (var x in streets)
                if (adress.Contains(x)) return true;
            return false;

        }




        public static VKParser vkApi = new VKParser();

        static void Main()
        {
            var reddit = new VKGroup("-188523184");
            Console.WriteLine("exist = " + reddit.exist);
            
            Console.WriteLine("tPosts = " + reddit.totalPosts);

            reddit.LoadPosts(10);

            Console.WriteLine("count = " + reddit.posts.Count);

            reddit.LoadNewPosts(2);

            reddit.LoadNewPosts();

            reddit.LoadNewPosts();
        }
    }
}