using Microsoft.AspNetCore.Mvc;
using Uniqlo.BL.Services.Abstractions;
using Uniqlo.DAL.Models;

namespace Uniqlo.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericCRUDService _service;

        public HomeController(IGenericCRUDService service)
        {
            _service = service;
        }
        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<SliderItem> sliderItems = await _service.GetAllAsync<SliderItem>();
            List<SliderItem> twoSliderItems = sliderItems.OrderByDescending(x => x.Id).Take(2).ToList();
            return View(twoSliderItems);
        }
    }
}

