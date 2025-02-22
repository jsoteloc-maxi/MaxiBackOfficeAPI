using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.Agent.Infrastructure.Entities;


namespace Maxi.BackOffice.Agent.Application.Contracts
{
    public interface IRpcService
    {
        GiactResult GiactValidation(GiactInquiry request);
        MaxiItemInfo GetItemInfo(ObjetoRequestItem aReq);
        CCAgFeeCommRes GetAgFeeCommInfo(int idAgent, decimal amount, int idState);
        CC_GetMakerByAccRes GetMakerByAcc(string rout, string acc);
        List<CheckType> GetCheckTypesList();
        AccountCheckSummary GetAccountCheckSummary(string rout, string acc);
        List<IssuerActionCheck> GetIssuerAction(int idIssuer);
        AccountCautionNotes GetAccountCautionNotes(string rout, string acc, string checkNum);
        List<CheckTiny> GetRecentChecksByCustomer(int idCustomer);
        List<CheckTiny> GetRecentChecksByIssuer(int idIssuer);
        PaginationResponse<List<CheckTiny>> GetRecentChecksByCustomer(int idIssuer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null);
        PaginationResponse<List<CheckTiny>> GetRecentChecksByIssuer(int idCustomer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrden = null);
        List<CheckTiny> GetChecksProcessedReport(DateTime date1, DateTime date2, string custName, string checkNum);
        List<CheckTiny> GetChecksRejectedReport(DateTime date1, DateTime date2, string custName, string checkNum, string printed);
        IRDResponse GetCheckIRD(int idCheck, string docType);
        string GetPcParam(string ident, string col);
        int SetPcParam(string ident, string col, string value);
        List<CheckElementEdited> GetCheckEditedElements(int idCheck);
        CheckTiny GetCheckByMircData(string ChNum, string RoutNum, string AccNum);
        int SetIdcheckImagePending();
        int CheckImageProcessed(int id, List<UploadCheck> uploadChecks);
        void UploadChecks(List<UploadCheck> uploadChecks);
        int DeleteCheckByIdCheckPending(int Id);
    }
}
