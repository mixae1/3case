using System;
using System.Collections.Generic;
using VkNet;

namespace SocNetParser
{
    /// <summary>
    /// Группа вк
    /// </summary>
    class VKGroup
    {
        public VKGroup(string domain)
        {
            try
            {
                var wallInfo = MainProgram.vkApi.GetWallInfo(new VKParser.ParserParams(domain));
                totalPosts = wallInfo.TotalCount;
                this.domain = domain;
                exist = true;
            }
            catch (Exception e)
            {
                exist = false;
                Console.WriteLine(e.Message);
            }

        }

        public string domain { get; private set; }
        public List<Post> posts { get; private set; }
        public ulong followers { get; private set; } //через статистику как нибудь надо получить
        public ulong totalPosts { get; private set; }
        public bool exist { get; private set; }

        public void LoadPosts(ulong counts)
        {
            posts = MainProgram.vkApi.GetPosts(new VKParser.ParserParams(domain, counts));
        }
    }
}
