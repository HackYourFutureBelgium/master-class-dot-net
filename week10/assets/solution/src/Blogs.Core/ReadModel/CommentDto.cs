namespace Blogs.Core.ReadModel;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public Guid AuthorId { get; set; }
    public Guid PostId { get; set; }
    public PostDto Post { get; set; }

    public static explicit operator CommentDto( Comment comment )
        => new()
        {
            Content = comment.Content,
            AuthorId = comment.AuthorId.Value,
            PostId = comment.PostId.Value,
            Post = ( PostDto )comment.Post
        };
}
