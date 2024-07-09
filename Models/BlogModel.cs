using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } // başlık

        [Required]
        public string Summary { get; set; } // özet

        [Required]
        public string Detail { get; set; } // içerik

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public IFormFile Image { get; set; }
        public string? ImgUrl { get; set; }

    }

    public class PostComment
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Comment { get; set; }

        [Required]
        public int PostId { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class PostModel
    {
        public int Id { get; set; }
        public int IsApproved { get; set; }
        public Post Post { get; set; }

        public List<PostComment> Comments { get; set; }
    }
}
