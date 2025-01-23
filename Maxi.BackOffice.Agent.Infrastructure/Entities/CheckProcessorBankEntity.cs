using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "CheckProcessorBank")]
    public class CheckProcessorBankEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdCheckProcessorBank { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public string Name { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public string ABACode { get; set; }

    }
}
