using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "CheckImagePending")]
    public class CheckImagePendingEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdcheckImagePending { get; set; }
        [PropEntityAtributes(Update = false)]
        public DateTime CreateDate { get; set; }
        [PropEntityAtributes(Update = false)]
        public int UserId { get; set; }
        [PropEntityAtributes(Update = false)]
        public int Agent { get; set; }
        [PropEntityAtributes(Insert = false, Update = true)]
        public DateTime ProcessingDate { get; set; }
        [PropEntityAtributes(Update = false)]
        public string Path { get; set; }
    }
}
