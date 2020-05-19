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
    public class AlbumController : Controller
    {
        private static List<Album> _albums = new List<Album>();

        public JsonResult Get(int id = -1)
        {
            //I would almost never do this in real life, however, the API has several areas where it's lacking, such as pagination, requesting a range, etc, 
            //therefore, because for this it seems overkill to do a full DB, I'll be caching to a static variable.
            if (_albums.Count == 0)
            {
                _albums = JsonConvert.DeserializeObject<List<Album>>(new WebClient().DownloadString($"{ConnectionStrings.JSON_REPO}/albums")); //Get all the albums
            }

            id = (id == -1) ? 1 : id; //Default to the first album
            id = (id > _albums.Count) ? 1 : id; //Did they loop too high?
            id = (id == 0) ? _albums.Count : id; //Did they go too low?

            Album album = _albums.Find(x => x.Id == id);

            if (album.AlbumOwner == null || album.AlbumOwner.Name == null)
            {
                var user = (User)new UserController().Get(album.UserId).Data;

                _albums.Find(x => x.Id == id).AlbumOwner = user;
            }

            if (album.AlbumOwner.UserPosts == null || album.AlbumOwner.UserPosts.Count == 0)
            {
                var posts = (List<UserPost>)new UserPostsController().Get(album.UserId).Data;
                _albums.Find(x => x.Id == id).AlbumOwner.UserPosts = posts;
            }

            if (album.Photos == null)
            {
                var photos = (List<Photo>)new PhotosController().Get(album.Id).Data;
                _albums.Find(x => x.Id == id).Photos = photos;
            }

            return Json(_albums.Find(x => x.Id == id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Search(string title = "", string userName = "")
        {
            //This will be an inclusive search to allow for more returns
            //rather than a restrictive. I'm assuming more hits = better in this case
            List<Album> matches = new List<Album>();

            if (title != "")
            {
                matches.AddRange(_albums.FindAll(x => x.Title.ToLower().Contains(title.ToLower().Trim()))); //Really simplistic 'contains'
            }

            if (userName != "")
            {
                List<int> userMatches = (List<int>)new UserController().Search(userName.ToLower()).Data;
                matches.AddRange(_albums.Where(x => userMatches.Contains(x.UserId)));
            }

            return Json(matches.Select(x => x.Id).Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}