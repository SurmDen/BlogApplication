using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Blog
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set;}

        [NotMapped]
        public string? ImageBase64String { get; set; }

        public string? VideoPath { get; set; }

        public string? VideoName { get; set; }

        [NotMapped]
        public string? TagList { get; set; }

        public string? Image { get; set; }

        public string Alias { get; set; }

        public DateTime DateOfPublish { get; set; }

        public DateTime DateOfUpdate { get; set; }

        public bool IsArchived { get; set; }

        public List<Section> Sections { get; set; }

        public User? User { get; set; }

        public long UserId { get; set; }

        public Language? Language { get; set; }

        public long LanguageId { get; set; }

        public Category? Category { get; set; }

        public long CategoryId { get; set; }

        public List<Tag>? Tags { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
