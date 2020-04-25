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
        VkApi vkApi;
        public VKParser()
        {
            vkApi = new VkApi();
            vkApi.Authorize(new ApiAuthParams
            {
                AccessToken = "70aed950fe8f6d4f096d08edaf28051f79635495c92f9dea5d771dff3348744167581215fc252ac804748"
            });
        }

        public class ParserParams
        {
            public string? Domain;
            public ulong Count;
        }

        public ulong GetCountOfPosts(ParserParams @params)
        {
            WallGetObject wall = vkApi.Wall.Get(
                new WallGetParams
                {
                    Domain = @params.Domain,
                    Count = @params.Count
                });
            return wall.TotalCount;
        }

        public class Body
        {
            public Body(int reps, int liks, int vies, int coms, DateTime date)
            {
                reposts = reps;
                likes = liks;
                views = vies;
                comments = coms;
                time = date;
            }
            public int reposts;
            public int likes;
            public int views;
            public int comments;
            public DateTime time;
        }

        public List<Body> GetPosts(ParserParams @params)
        {
            WallGetObject wall = vkApi.Wall.Get(
                new WallGetParams{
                    Domain = @params.Domain,
                    Count = @params.Count 
                });

            return new List<Body>(
                wall.WallPosts.Select(x=> 
                new Body(
                    x.Reposts.Count, 
                    x.Likes.Count, 
                    x.Views.Count, 
                    x.Comments.Count, 
                    x.Date.Value
                    )));

        }
    }
}
