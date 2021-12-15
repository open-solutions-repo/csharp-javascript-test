namespace Xnet.Store5.Api.Interfaces
{
    public interface IDocumentRepositoryFactory
    {
        IDocumentRepository Create(string dbType);
    }
}
