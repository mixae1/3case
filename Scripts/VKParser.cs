using System;
using System.Collections.Generic;
using System.Linq;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace SocNetParser
{
    class VKParser
    {
        VkApi api;

        public VKParser()
        {
            api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                AccessToken = Properties.Resources.VKtoken
            });
        }


        public class ParserParams
        {
            public ParserParams(string domain = "", ulong count = 0)
            {
                Domain = domain;
                Count = count;
            }
            public string Domain;
            public ulong Count;
        }

        public WallGetObject GetWallInfo(ParserParams @params)
        {
            try
            {
                long groupId;
                var w_pars = new WallGetParams();
                if (long.TryParse(@params.Domain, out groupId)) w_pars.OwnerId = groupId;
                else w_pars.Domain = @params.Domain;
                WallGetObject wall = api.Wall.Get(
                    new WallGetParams
                    {
                        OwnerId = groupId,
                        Count = 0
                    });
                return wall;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public List<Post> GetPosts(ParserParams @params, ulong offset = 0)
        {
            List<Post> posts = new List<Post>();
            try
            {
                long groupId;
                WallGetObject wall;
                var w_pars = new WallGetParams();
                if (long.TryParse(@params.Domain, out groupId)) w_pars.OwnerId = groupId;
                else w_pars.Domain = @params.Domain;

                for (ulong i = 0; i < @params.Count; i += 100)
                {
                    w_pars.Count = Math.Min(@params.Count - i, 100);
                    w_pars.Offset = offset + i;
                    wall = api.Wall.Get(w_pars);
                    try
                    {
                        posts.AddRange(wall.WallPosts.Select(x =>
                            new Post(
                                x.Reposts.Count,
                                x.Likes.Count,
                               // x.Views.Count,
                                x.Comments.Count,
                                x.Date.Value
                                )));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return posts;
        }

        public List<Post> GetPostsBeforeDT(ParserParams @params, DateTime time, ulong offset = 0)
        {
            List<Post> posts = new List<Post>();
            try
            {
                long groupId;
                WallGetObject wall;
                var w_pars = new WallGetParams();
                if (long.TryParse(@params.Domain, out groupId)) w_pars.OwnerId = groupId;
                else w_pars.Domain = @params.Domain;
                w_pars.Count = 100;

                for (ulong i = 0;; i += 100)
                {
                    w_pars.Offset = offset + i;
                    wall = api.Wall.Get(w_pars);

                    if (wall.WallPosts.Count == 0) return posts;
                    foreach (var p in wall.WallPosts)
                    {
                        if (p.Date <= time) return posts;
                        posts.Add(
                            new Post(
                                p.Reposts == null ? 0 : p.Reposts.Count,
                                p.Likes == null ? 0 : p.Likes.Count,
                                p.Views == null ? 0 : p.Views.Count,
                                p.Comments == null ? 0 : p.Comments.Count,
                                p.Date.Value
                                ));
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return posts;
        }
    }
}
