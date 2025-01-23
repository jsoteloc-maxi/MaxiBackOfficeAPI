using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "FeeChecks")]
    public class FeeChecksEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdFeeChecks { get; set; }

        [PropEntityAtributes()]
        public int IdAgent { get; set; }

        [PropEntityAtributes()]
        public bool AllowChecks { get; set; } = false;

        [PropEntityAtributes()]
        public string FeeName { get; set; } = "";

        [PropEntityAtributes()]
        public decimal TransactionFee { get; set; } = 0;

        [PropEntityAtributes()]
        public decimal ReturnCheckFee { get; set; } = 0;

        [PropEntityAtributes()]
        public DateTime? DateOfLastChange { get; set; }

        [PropEntityAtributes()]
        public int EnterByIdUser { get; set; }

        [PropEntityAtributes()]
        public decimal FeeCheckScanner { get; set; } = 0;
    }
}
