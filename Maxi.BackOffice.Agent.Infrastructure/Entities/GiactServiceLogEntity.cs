using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "GiactServiceLog")]
    public class GiactServiceLogEntity : IEntityType
    {        

        [PropEntityAtributes( Key = true , Insert = false , Update =false )]
        public int Id { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public string UniqueId { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public DateTime DateRecord { get; set; }

        [PropEntityAtributes(Insert = false, Update = true)]
        public bool StError { get; set; }

        [PropEntityAtributes(Insert = false, Update = true)]
        public string Error { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public string RequestJSON { get; set; }

        [PropEntityAtributes(Insert = false, Update = true)]
        public string ResponseJSON { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public int IdAgent { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public int IdUser { get; set; }
    }
}
