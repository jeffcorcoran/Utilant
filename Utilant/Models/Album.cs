using System.Collections.Generic;
using System.Linq;

namespace Utilant.Models
{
    public class Album
    {
        public int Id { get; set; }
        public int UserId
        {
            get { return (AlbumOwner != null) ? AlbumOwner.Id : -1; } //Probably a better way of doing this, but trying to prove a point elsewhere
            set { AlbumOwner = new User { Id = value }; }
        }
        public User AlbumOwner { get; set; }
        public string Title { get; set; }
        public List<Photo> Photos { get; set; }
        public Photo Thumbnail
        {
            get { return (Photos != null) ? Photos.OrderBy(x => x.Id).First() : new Photo(); } //If thumbnail was used a ton, I could make a static bool to check for the loaded status so it wasn't iterating through the linq query every single time
        }
    }
}