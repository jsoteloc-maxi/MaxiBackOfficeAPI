namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    public class SpCC_GetAccountCheckSummaryEntity
    {
        public int IdIssuer { get; set; }
        public string IssuerName { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalRejected { get; set; }
        public DateTime? LastRejectionDate { get; set; }
        public string LastRejectionReason { get; set; }

    }
}
