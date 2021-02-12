using FoodPal.Providers.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodPal.Providers.Services.Contracts
{
    public interface ICatalogueItemService
    {
        Task<IEnumerable<CatalogueItemDto>> GetCatalogueItemsForProvider(int providerId);

        Task<CatalogueItemDto> GetCatalogueItemById(int catalogueItemId);

        Task<bool> CatalogueItemExists(string catalogueItemName, int providerId);

        Task<int> Create(NewCatalogueItemDto catalogueItem);

        Task Update(CatalogueItemDto catalogueItem);

        Task Delete(int catalogueItemId);
    }
}
