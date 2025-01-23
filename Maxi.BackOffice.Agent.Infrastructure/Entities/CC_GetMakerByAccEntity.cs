using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    public class CC_GetMakerByAccEntity : IEntityType
    {
        public int Maker_ID  {get;set; }

        public DateTime MAK_DateCreated { get; set; }

        public int MAK_IdUserCreated { get; set; }

        public string MAK_Name { get; set; } = "";

        public string MAK_Address { get; set; } = "";

        public string MAK_City { get; set; } = "";

        public string MAK_State { get; set; } = "";

        public int IdState { get; set; }

        public string MAK_ZipCode { get; set; } = "";

        public bool MAK_Active { get; set; } = false;
    }
}
