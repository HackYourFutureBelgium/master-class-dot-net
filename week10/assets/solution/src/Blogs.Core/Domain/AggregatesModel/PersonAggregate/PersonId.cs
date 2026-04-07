namespace Blogs.Core.Domain.AggregatesModel.PersonAggregate;

[ JsonConverter( typeof( IdentityJsonConverter< PersonId > ) ) ]
[ TypeConverter( typeof( IdentityTypeConverter< PersonId > ) ) ]
public class PersonId : Identity;
