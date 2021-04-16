using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class AccountEditModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Tile { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public int? Speccializeid { get; set; }
        public int? LevelId { get; set; }
        public bool? OnReady { get; set; }
        public int? FormOnWorkId { get; set; }
        public string AvatarUrl { get; set; }
    }
}
