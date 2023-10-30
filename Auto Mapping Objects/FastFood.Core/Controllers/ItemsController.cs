using Microsoft.AspNetCore.Mvc;
using FastFood.Web.ViewModels.Items;
using FastFood.Services.Data;

namespace FastFood.Web.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemService itemService;
        public ItemsController(IItemService itemService)
        {
            this.itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<CreateItemViewModel> availableCategories =
               await itemService.GetAllAvailableCategoriesASync();
            return View(availableCategories.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateItemInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            await itemService.CreateAsync(model);

            return RedirectToAction("All");
        }

        public async Task<IActionResult> All()
        {
            IEnumerable<ItemsAllViewModels> items = 
                await itemService.GetAllAsync();

            return View(items.ToList());
        }   
    }
}
