using AutoMapper;
using FoodPal.Providers.Dtos;
using FoodPal.Providers.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FoodPal.Providers.API.Controllers
{
    [Route("api/providers/{providerId}/menu")]
    [ApiController]
    public class CatalogueItemsController : ControllerBase
    {
        private readonly ICatalogueItemService _catalogueItemService;
        private readonly IProviderService _providerService;
        private readonly IMapper _mapper;

        public CatalogueItemsController(ICatalogueItemService catalogueItemService, IProviderService providerService, IMapper mapper)
        {
            _catalogueItemService = catalogueItemService ?? throw new ArgumentNullException(nameof(catalogueItemService));
            _providerService = providerService ?? throw new ArgumentNullException(nameof(providerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        public async Task<IActionResult> GetCatalogueItems(int providerId)
        {
            try
            {
                if (await _providerService.GetByIdAsync(providerId) == null)
                    return NotFound();

                var items = await _catalogueItemService.GetCatalogueItemsForProviderAsync(providerId);
                return Ok(items);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpGet("{itemId}", Name = "GetCatalogueItem")]
        public async Task<IActionResult> GetCatalogueItem(int providerId, int itemId)
        {
            try
            {
                if (await _providerService.GetByIdAsync(providerId) == null)
                    return NotFound();


                var item = await _catalogueItemService.GetCatalogueItemByIdAsync(itemId);

                if (item == null)
                    return NotFound();

                return Ok(item);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCatalogueItem(int providerId,
            NewCatalogueItemDto catalogueItem)
        {
            try
            {
                if (await _providerService.GetByIdAsync(providerId) == null)
                    return NotFound();

                if (await _catalogueItemService.CatalogueItemExistsAsync(catalogueItem.Name, providerId))
                {
                    ModelState.AddModelError(
                        "Name",
                        "A provider with the same name already exists into the database");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var insertedId = await _catalogueItemService.CreateAsync(catalogueItem);

                if (insertedId == 0)
                    return Problem();

                return CreatedAtRoute("GetCatalogueItem",
                    new
                    {
                        itemId = insertedId,
                        providerId = providerId
                    },
                    insertedId);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateCatalogueItem(int providerId, int itemId,
            CatalogueItemDto item)
        {
            try
            {
                if (item.Id != itemId)
                {
                    ModelState.AddModelError(
                        "Identifier",
                        "Request body not apropiate for ID");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _providerService.GetByIdAsync(providerId) == null)
                {
                    return NotFound();
                }

                if (await _catalogueItemService.GetCatalogueItemByIdAsync(itemId) == null)
                {
                    return NotFound();
                }

                await _catalogueItemService.UpdateAsync(item);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteCatalogueItem(int providerId, int itemId)
        {
            try
            {
                if (await _providerService.GetByIdAsync(providerId) == null)
                {
                    return NotFound();
                }

                if (await _catalogueItemService.GetCatalogueItemByIdAsync(itemId) == null)
                {
                    return NotFound();
                }

                await _catalogueItemService.DeleteAsync(itemId);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }
    }
}
