using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

using Utilant.Models;

using Newtonsoft.Json;

namespace Utilant.Controllers
{
    public class PhotosController : Controller
    {
        private static Dictionary<int, List<Photo>> _albumPhotos = new Dictionary<int, List<Photo>>();

        [HttpGet]
        public JsonResult Get(int id)
        {
            if (!_albumPhotos.ContainsKey(id))
            {
                _albumPhotos.Add(id, JsonConvert.DeserializeObject<List<Photo>>(new WebClient().DownloadString($"{ConnectionStrings.JSON_REPO}/album/{id}/photos")));
            }

            return Json(_albumPhotos[id]);
        }
    }
}