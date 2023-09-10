using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Post.Query.Domain.Entities;

[Table("Comment")]
public class CommentEntity
{
    [Key]
    public Guid CommentId { get; set; }
    public string Username { get; set; } = null!;
    public DateTime CommentDate { get; set; }
    public string Comment { get; set; } = null!;
    public bool Edited { get; set; }
    public Guid PostId { get; set; }

    // to eliminate the circular reference
    [JsonIgnore]
    public virtual PostEntity Post { get; set; } = null!;
}