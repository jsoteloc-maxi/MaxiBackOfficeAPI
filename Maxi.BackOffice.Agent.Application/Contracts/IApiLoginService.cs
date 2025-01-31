namespace Maxi.BackOffice.Agent.Application.Contracts
{
    public interface IApiLoginService : ICustomServiceBase
    {
        dynamic AutenticateSessionData(dynamic r, string userName, string lastName);
    }
}
