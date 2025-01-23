using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "Lenguages")]
    public class LanguageEntity
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdLenguage { get; set; }

        [PropEntityAtributes( Insert = true, Update = true)]
        public string Name { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public string Culture { get; set; }
    }
}
