namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

[ JsonConverter( typeof( IdentityJsonConverter< PostId > ) ) ]
[ TypeConverter( typeof( IdentityTypeConverter< PostId > ) ) ]
public class PostId : Identity;
