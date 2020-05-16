using System.Text.RegularExpressions;

namespace Utilant.Models
{
    public class UserPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string HtmlBody
        {
            get { return Regex.Replace(Body, @"\n", "<br />"); }
        }
    }
}