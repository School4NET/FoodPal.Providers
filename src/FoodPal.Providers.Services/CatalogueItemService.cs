using AutoMapper;
using FoodPal.Providers.DataAccess.UnitOfWork;
using FoodPal.Providers.Dtos;
using FoodPal.Providers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodPal.Providers.Services
{
    class CatalogueItemService : ICatalogueItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CatalogueItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        public async Task<int> Create(NewCatalogueItemDto catalogueItem)
        {
            var itemModel = _mapper.Map<NewCatalogueItemDto, DomainModels.CatalogueItem>(catalogueItem);

            var catalogue = await _unitOfWork.CatalogueRepository.SingleOrDefaultAsync(x => x.Id == catalogueItem.CatalogueId);

            itemModel.Catalogue = catalogue;

            await _unitOfWork.CatalogueItemsRepository.AddAsync(itemModel);
            await _unitOfWork.CommitAsync();
            return itemModel.Id;
        }


        public async Task Delete(int catalogueItemId)
        {
            var itemModel = await _unitOfWork.CatalogueItemsRepository.GetWithProviderByIdAsync(catalogueItemId);

            _unitOfWork.CatalogueItemsRepository.Remove(itemModel);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<CatalogueItemDto>> GetCatalogueItemsForProvider(int providerId)
        {
            var models = await _unitOfWork.CatalogueItemsRepository
                .GetAllWithProviderAsync(providerId);

            return _mapper.Map<IEnumerable<DomainModels.CatalogueItem>, IEnumerable<CatalogueItemDto>>(models);
        }

        public async Task<CatalogueItemDto> GetCatalogueItemById(int catalogueItemId)
        {
            var model = await _unitOfWork.CatalogueItemsRepository
                .GetWithProviderByIdAsync(catalogueItemId);

            return _mapper.Map<DomainModels.CatalogueItem, CatalogueItemDto>(model);
        }

        public async Task<bool> CatalogueItemExists(string catalogueItemName, int providerId)
        {

            var itemFound = await _unitOfWork.CatalogueItemsRepository
            .SingleOrDefaultAsync(x =>
            x.Name.ToLower() == catalogueItemName.ToLower() && x.Catalogue.ProviderId == providerId);

            return itemFound != null;
        }


        public async Task Update(CatalogueItemDto catalogueItem)
        {
            await _unitOfWork.CommitAsync();
        }
    }
}
