using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utilant.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public Uri FullSizeUrl { get; set; }
        public Uri ThumbnailUrl { get; set; }
    }
}