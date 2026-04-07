namespace Blogs.Core.Domain.AggregatesModel.BlogAggregate;

[ JsonConverter( typeof( IdentityJsonConverter< BlogId > ) ) ]
[ TypeConverter( typeof( IdentityTypeConverter< BlogId > ) ) ]
public class BlogId : Identity;
