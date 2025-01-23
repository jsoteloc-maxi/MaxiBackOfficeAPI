namespace Maxi.BackOffice.Agent.Infrastructure.Contracts
{
    public interface IPcParamsRepository
    {

        string GetParam(string ident, string col);
        int SetParam(string ident, string col, string value);
    }
}
