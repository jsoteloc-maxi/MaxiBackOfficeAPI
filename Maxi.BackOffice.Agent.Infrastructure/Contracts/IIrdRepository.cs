namespace Maxi.BackOffice.Agent.Infrastructure.Contracts
{
    public interface IIrdRepository
    {
        dynamic ObtenerImagenesIRD(int idCheckOri, bool ignoreDeny = false, bool IsIrdPrinted = true);

        dynamic ObtenerImpresionIRD(int idCheckOri, string logoImg);

        dynamic ObtenerImpresionIRDParams(int idCheckOrig, string logoFile);

    }
}
