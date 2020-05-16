using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;

using Utilant.Models;

namespace Utilant.Controllers
{
    public class UserPostsController : Controller
    {
        private static Dictionary<int, List<UserPost>> _posts = new Dictionary<int, List<UserPost>>();

        public JsonResult Get(int id)
        {
            if (!_posts.ContainsKey(id))
            {
                _posts.Add(id, JsonConvert.DeserializeObject<List<UserPost>>(new WebClient().DownloadString($"{ConnectionStrings.JSON_REPO}/posts?userId={id}")));
            }

            return Json(_posts[id]);
        }
    }
}