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
            public string? Domain;
            public ulong Count;
        }

        public WallGetObject GetWallInfo(ParserParams @params)
        {
            try
            {
                WallGetObject wall = api.Wall.Get(
                    new WallGetParams
                    {
                        Domain = @params.Domain,
                        Count = @params.Count
                    });
                return wall;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public List<Post> GetPosts(ParserParams @params)
        {
            List<Post> posts = new List<Post>();
            try
            {
                for (ulong i = 0; i < @params.Count; i += 100)
                {

                    WallGetObject wall = api.Wall.Get(
                        new WallGetParams
                        {
                            Domain = @params.Domain,
                            Count = Math.Min(@params.Count - i, 100),
                            Offset = i
                        });
                    posts.AddRange(wall.WallPosts.Select(x =>
                        new Post(
                            x.Reposts.Count,
                            x.Likes.Count,
                            x.Views.Count,
                            x.Comments.Count,
                            x.Date.Value
                            )));
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
