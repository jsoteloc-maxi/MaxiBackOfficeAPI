using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "OrbographLog")]
    public class OrbographLogEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int Id { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public DateTime DateRecord { get; set; }


        [PropEntityAtributes(Insert = false, Update = true)]
        public string Exception { get; set; } = "";

        [PropEntityAtributes(Insert = true, Update = false)]
        public string RequestJSON { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public string ResponseJSON { get; set; } = "";

        [PropEntityAtributes(Insert = true, Update = false)]
        public int IdAgent { get; set; }

        [PropEntityAtributes(Insert = true, Update = false)]
        public int IdUser { get; set; }

        [PropEntityAtributes(Insert = false, Update = true)]
        public int ErrorCode { get; set; }

        [PropEntityAtributes(Insert = false, Update = true)]
        public string ErrorCodeName { get; set; } = "";


        [PropEntityAtributes(Insert = false, Update = true)]
        public string Micr { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public string MicrAmount { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public int MicrAmountScore { get; set; } = 0;

        [PropEntityAtributes(Insert = false, Update = true)]
        public string EPC { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public string Routing { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public string Account { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public string CheckNum { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public string FieldCarValue { get; set; } = "";

        [PropEntityAtributes(Insert = false, Update = true)]
        public int FieldCarScore { get; set; }
    }
}
