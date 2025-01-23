using Maxi.BackOffice.CrossCutting.Common.Attributes;

namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    [EntityAtributes(Schema = "dbo", Tablename = "Checks")]
    public class ChecksEntity : IEntityType
    {
        [PropEntityAtributes(Key = true, Insert = false, Update = false)]
        public int IdCheck { get; set; }

        public int IdCustomer { get; set; }
        public string Name { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }
        public string CustomerName { get => (Name ?? "") + " " + ((FirstLastName ?? "")) + " " + (SecondLastName ?? ""); }

        public int IdIdentificationType { get; set; }
        public string IdentificationType { get; set; }

        public string CheckNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string Account { get; set; }

        public int IdIssuer { get; set; }
        public string IssuerName { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public decimal Amount { get; set; }

        public int IdStatus { get; set; }
        public string StatusName { get; set; }

        public DateTime? DateOfMovement { get; set; }

        public int IdAgent { get; set; }
        public string AgentName { get; set; }

        public string MicrAuxOnUs { get; set; }
        public string MicrRoutingTransitNumber { get; set; }
        public string MicrOnUs { get; set; }
        public string MicrAmount { get; set; }
        public string MicrOriginal { get; set; }
        public string MicrManual { get; set; }

        public int IdCheckProcessorBank { get; set; }

    }
}
