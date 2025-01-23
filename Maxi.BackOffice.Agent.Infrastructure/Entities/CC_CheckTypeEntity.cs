using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "CC_CheckType")]
    public class CC_CheckTypeEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdCheckType { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public DateTime CT_DateCreated { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public string CT_Name { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public bool CT_Active { get; set; }
    }
}
