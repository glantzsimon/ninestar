using K9.WebApplication.ViewModels.MobileApi;

namespace K9.WebApplication.Services
{
    public interface IMobileAdminDataService
    {
        object Catalog();
        object Schema(string entity);
        object List(string entity, int afterId, int limit);
        object Find(string entity, int id);
        object Save(string entity, MobileAdminWriteRequest request);
        void Delete(string entity, int id);
        object Selection(string relationship, int ownerId);
        object ReplaceSelection(string relationship, MobileAssociationRequest request);
    }
}
