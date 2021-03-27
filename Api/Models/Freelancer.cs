using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Freelancer
    {
        public int UserId { get; set; }
        public int? Speccializeid { get; set; }
        public int? LevelId { get; set; }
        public byte? OnReady { get; set; }
        public int? FormOnWorkId { get; set; }
    }
}
