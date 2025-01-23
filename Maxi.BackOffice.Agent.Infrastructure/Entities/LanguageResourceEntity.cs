using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "LenguageResource")]
    class LanguageResourceEntity :IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = true, Update = false)]
        public int IdLenguageResource { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public int IdLenguage { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public string MessageKey { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public string Message { get; set; }
    }
}
