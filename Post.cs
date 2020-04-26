using System;
using System.Collections.Generic;
using System.Text;

namespace SocNetParser
{
    /// <summary>
    /// Информация о посте в соц. сети
    /// </summary>
    class Post
    {
        public Post(int reps, int liks, int vies, int coms, DateTime date)
        {
            reposts = reps;
            likes = liks;
            views = vies;
            comments = coms;
            time = date;
        }
        public int reposts { get; private set; }
        public int likes { get; private set; }
        public int views { get; private set; }
        public int comments { get; private set; }
        public DateTime time { get; private set; }
    }
}
