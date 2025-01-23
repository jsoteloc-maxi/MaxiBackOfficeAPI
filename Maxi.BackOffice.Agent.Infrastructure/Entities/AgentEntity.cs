using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "Agent")]
    public class AgentEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdAgent { get; set; }

        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string AgentAddress { get; set; }
        public string AgentCity { get; set; }
        public string AgentState { get; set; }
        public string AgentZipCode { get; set; }
        public string AgentPhone { get; set; }
        public string AgentEmail { get; set; }

    }
}
