namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

[ JsonConverter( typeof( IdentityJsonConverter< CommentId > ) ) ]
[ TypeConverter( typeof( IdentityTypeConverter< CommentId > ) ) ]
public class CommentId : Identity;
