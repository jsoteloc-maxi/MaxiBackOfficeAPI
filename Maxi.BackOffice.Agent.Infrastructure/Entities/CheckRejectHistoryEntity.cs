using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "CheckRejectHistory")]
    public class CheckRejectHistoryEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdCheckRejectHistory { get; set; }

        public int IdCheck { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public int IdReturnedReason { get; set; }
        public DateTime? DateOfReject { get; set; }
        public int EnterByIdUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? DateofLastChange { get; set; }
        public bool IrdPrinted { get; set; }
        public string IrdMicr { get; set; } = "";
    }
}
