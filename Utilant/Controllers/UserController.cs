using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Utilant.Models;

using Newtonsoft.Json;


namespace Utilant.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private static List<User> _users = new List<User>();

        public JsonResult Get(int id)
        {
            if (_users.Count == 0)
            {
                _users = JsonConvert.DeserializeObject<List<User>>(new WebClient().DownloadString($"{ConnectionStrings.JSON_REPO}/users"));
            }

            return Json(_users.FirstOrDefault(x => x.Id == id));
        }

        public JsonResult Search(string userName)
        {
            //Can't really search well on the API side of things 
            //because they require exact matches and searching on multiple params 
            //would be more restrictive, and we don't want to do 30 gets, 
            //so get all the things

            if (_users.Count == 0) //This is repeated, therefore, I should make a helper method, let's see if I remember
            {
                _users = JsonConvert.DeserializeObject<List<User>>(new WebClient().DownloadString($"{ConnectionStrings.JSON_REPO}/users"));
            }

            List<User> matches = new List<User>();

            string[] names = userName.Split(' '); //If someone did a first name, last name, do a split

            foreach (string name in names)
            {
                matches.AddRange(_users.FindAll(x => x.Name.ToLower().Contains(name.ToLower())));
                matches.AddRange(_users.FindAll(x => x.Username.ToLower().Contains(name.ToLower())));
            }


            return Json(matches.Select(x => x.Id).Distinct().ToList()); //Return ints that match
        }
    }
}