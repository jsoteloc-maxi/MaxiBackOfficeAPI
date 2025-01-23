using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "CC_ReturnedReasons")]
    public class CC_ReturnedReasonsEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int ReturnedReason_ID { get; set; }

        public string RTR_Name { get; set; }
        public string RTR_ASCX9 { get; set; }
        public string RTR_X9ShortName { get; set; }
        public string RTR_X9Abbreviation { get; set; }

    }
}
