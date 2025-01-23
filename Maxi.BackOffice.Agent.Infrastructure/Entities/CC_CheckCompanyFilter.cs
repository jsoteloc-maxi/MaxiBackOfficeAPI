using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{

    [EntityAtributes(Schema = "dbo", Tablename = "CheckCompanyWordFilter")]
    public class CC_CheckCompanyFilter : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdCheckCompanyWordFilter { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public string Word { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public int EnterByIdUser { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public DateTime CreationDate { get; set; }
     
    }
}
