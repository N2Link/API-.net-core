using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
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
