using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "CC_AgFees")]
    public class CC_AgFeesEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdAgCustFee { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public DateTime ACF_DateCreated { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public int ACF_IdUserCreated { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public int IdAgent { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public int IdCheckType { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public decimal ACF_CheckAmountFrom { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public decimal ACF_CheckAmountTo { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public decimal ACF_FeeFixed { get; set; }

        [PropEntityAtributes(Insert = true, Update = true)]
        public decimal ACF_FeePerc { get; set; }

    }
}
