using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "CC_AccVerifByAg")]
    public class CC_AccVerifByAgEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdAccVerifByAg { get; set; }

        [PropEntityAtributes()]
        public DateTime DateCreated { get; set; }

        [PropEntityAtributes()]
        public string Routing { get; set; }

        [PropEntityAtributes()]
        public string Account { get; set; }

        [PropEntityAtributes()]
        public string CheckNum { get; set; }

        [PropEntityAtributes()]
        public int IdCheck { get; set; }

        [PropEntityAtributes()]
        public int IdUser { get; set; }

        [PropEntityAtributes()]
        public int IdAgent { get; set; }

        [PropEntityAtributes()]
        public int IdLog { get; set; }

        [PropEntityAtributes()]
        public int IdIssuer { get; set; }

        [PropEntityAtributes()]
        public string Provider { get; set; }

        [PropEntityAtributes()]
        public decimal VerificationFee { get; set; }
    }
}
