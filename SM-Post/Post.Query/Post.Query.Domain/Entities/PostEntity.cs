using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Query.Domain.Entities;

[Table("Post")]
public class PostEntity
{
    [Key]
    public Guid PostId { get; set; }
    public string Author { get; set; } = null!;
    public DateTime DatePosted { get; set; }
    public string Message { get; set; } = null!;
    public int Likes { get; set; }

    // Navigation Property
    public virtual ICollection<CommentEntity>? Comments { get; set; }

}
