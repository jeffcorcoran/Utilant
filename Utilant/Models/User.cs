using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Utilant.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; } //This should probably be a class of its own rather than a string
        [JsonProperty("address")]
        public Address HomeAddress { get; set; }
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; } //This should probably be a class and not a string
        public Uri Website { get; set; }
        [JsonProperty("company")]
        public Company UserCompany { get; set; }
        public List<UserPost> UserPosts { get; set; }
    }
}