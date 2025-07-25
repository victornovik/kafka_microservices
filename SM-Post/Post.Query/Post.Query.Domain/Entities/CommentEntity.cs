﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Post.Query.Domain.Entities;

[Table("Comment", Schema = "dbo")]
public class CommentEntity
{
    [Key]
    public Guid CommentId { get; set; }
    public string UserName { get; set; }
    public DateTime CommentDate { get; set; }
    public string Comment { get; set; }
    public bool IsEdited { get; set; }
    public Guid PostId { get; set; }

    [JsonIgnore]
    public virtual PostEntity Post { get; set; }
}