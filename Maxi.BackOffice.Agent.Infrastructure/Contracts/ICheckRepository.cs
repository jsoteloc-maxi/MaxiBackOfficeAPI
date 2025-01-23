using Maxi.BackOffice.Agent.Infrastructure.Entities;

namespace Maxi.BackOffice.Agent.Infrastructure.Contracts
{

    public interface ICheckRepository
    {
        CCAgFeeCommResEntity GetCCAgFeeComm(int IdAgente, decimal CheckAmount, int State);
        
        CC_GetMakerByAccEntity GetMakerByAcc(string RoutingNum, string AccountNum);

        List<CC_CheckTypeEntity> GetAllCheckTypes();

        List<CC_CheckCompanyFilter> GetAllWordFilter();

        /// <summary>
        /// Obtiene notas de precaucion
        /// </summary>
        List<SpCC_GetCautionNotesEntity> GetCautionNotes(string rout, string acc, string checkNum);

        SpCC_GetAccountCheckSummaryEntity GetAccountCheckSummary(string rout, string acc);

        List<CC_IssuerActionCheck>  GetIssuerAction(int idIssuer);

        dynamic GetRecentChecksByCustomer(int idCustomer);
        dynamic GetRecentChecksByCustomer(int idCustomer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null);
        dynamic GetRecentChecksByIssuer(int idIssuer);
        dynamic GetRecentChecksByIssuer(int idIssuer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null);

        dynamic GetChecksProcessedReport(DateTime date1, DateTime date2, string custName, string checkNum);
        dynamic GetChecksRejectedReport(DateTime date1, DateTime date2, string custName, string checkNum, string printed);

        dynamic GetCheckEditedElements(int idCheck);

        /*07-Sep-2021*/
        /*UCF*/
        /*TSI_MAXI_013*/
        /*Se declara metodo en interfaz que verifica si existe en la DB un cheque con los datos de parametros*/
        dynamic GetCheckByMircData(String ChNum, String RoutNum, String AccNum);
    }
}
