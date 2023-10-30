using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.Models;
using Microsoft.AspNetCore.Mvc;
using FastFood.Web.ViewModels.Positions;
using FastFood.Services.Data;

namespace FastFood.Web.Controllers
{
    public class PositionsController : Controller
    {
        private readonly IPositionsService positionsService;

        public PositionsController(IPositionsService positionService)
        {
            this.positionsService = positionService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePositionInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            await positionsService.CreateAsync(model);

            return RedirectToAction("All", "Positions");
        }

        public async Task<IActionResult> All()
        {
           IEnumerable<PositionsAllViewModel> positions = await positionsService.GetAllAsync();

            return View(positions.ToList());
        }
    }
}
