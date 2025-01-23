using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "Users")]
    public class UsersEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdUser { get; set; }

        [PropEntityAtributes()]
        public string UserName { get; set; }

        [PropEntityAtributes()]
        public string UserLogin { get; set; }

        [PropEntityAtributes()]
        public string FirstName { get; set; }

        [PropEntityAtributes()]
        public string LastName { get; set; }

        [PropEntityAtributes()]
        public string SecondLastName { get; set; }
    }
}
