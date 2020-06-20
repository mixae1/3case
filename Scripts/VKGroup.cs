using System;
using System.Collections.Generic;
using System.Linq;

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
                var wallInfo = MainProgram.vkApi.GetWallInfo(new VKParser.ParserParams(domain, 2));
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

        public void ClearPosts()
        {
            posts = null;
        }

        public string domain { get; private set; }
        public List<Post> posts { get; private set; }
        //public ulong followers { get; private set; } //через статистику как нибудь надо получить
        public ulong totalPosts { get; private set; }
        public bool exist { get; private set; }

        public void LoadPosts(ulong counts)
        {
            if (!exist) return;
            posts = MainProgram.vkApi.GetPosts(new VKParser.ParserParams(domain, counts));
        }
        public void LoadNewPosts(ulong offset = 0)
        {
            if (posts == null)
            {
                LoadPosts(100);
                return;
            }
            if (!exist) return;
            List<Post> buf;
            (buf = MainProgram.vkApi.GetPostsBeforeDT(new VKParser.ParserParams(domain, 100), posts[0].time, offset)).AddRange(posts);
            posts = buf;
            totalPosts += (ulong)buf.Count;
        }
        public DateTime DTofLastTimePosted()
        {
            if (posts != null) return posts[0].time;
            else return DateTime.MinValue;
        }
        //!!!!
        public DateTime DTofLastTimePostedWithWall()
        {
            if (posts != null) return posts.LastOrDefault().time;
            else return DateTime.MinValue;
        }

    }
}
