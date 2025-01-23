using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "GlobalAttributes")]
    class GlobalAttributesEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = true, Update = false)]
        public string Name { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public string Value { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public string Description { get; set; }
    }
}
