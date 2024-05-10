﻿using LiteDB;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Text.Json.Serialization;

namespace NightMovie.API.Model
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        [JsonIgnore]
        public string? password { get; set; }
        [JsonIgnore]
        public bool IsAdmin { get; set; } = false;
        public string? UrlProfilPicture { get;set; }

        public float Weight { get; set; } = 1;

    }
}
