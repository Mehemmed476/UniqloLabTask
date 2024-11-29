using Microsoft.AspNetCore.Mvc;
using Uniqlo.BL.Services.Abstractions;
using Uniqlo.DAL.DTOs.ItemDTOs;
using Uniqlo.DAL.Models;

namespace Uniqlo.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderItemController : Controller
    {
        private readonly IGenericCRUDService _service;
        IWebHostEnvironment _webHostEnvironment;

        public SliderItemController(IGenericCRUDService service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            IEnumerable<SliderItem> sliderItems = await _service.GetAllAsync<SliderItem>();
            List<SliderItem> activeSliderItems = sliderItems.Where(s => !s.IsDeleted).ToList();
            return View(activeSliderItems);
        }

        [HttpGet]

        public async Task<IActionResult> Trash()
        {
            IEnumerable<SliderItem> sliderItems = await _service.GetAllAsync<SliderItem>();
            List<SliderItem> deactiveSliderItems = sliderItems.Where(s => s.IsDeleted).ToList();
            return View(deactiveSliderItems);
        }

        public async Task<IActionResult> Details(int id)
        {
            SliderItem sliderItem = await _service.GetByIdAsync<SliderItem>(id);
            return View(sliderItem);
        }
        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(SliderItemDto sliderItemDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please, choose a valid slider item");
                return View(sliderItemDto);
            }

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "upload", "sliderItems");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderItemDto.ImageFile.FileName);
            string fullPath = Path.Combine(path, fileName);

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                sliderItemDto.ImageFile.CopyTo(fileStream);
            }

            sliderItemDto.ImageUrl = fileName;

            SliderItem sliderItem = new SliderItem()
            {
                Title = sliderItemDto.Title,
                ButtonUrl = sliderItemDto.ButtonUrl,
                ImageUrl = sliderItemDto.ImageUrl
            };

            await _service.CreateAsync(sliderItem);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            SliderItem sliderItem = await _service.GetByIdAsync<SliderItem>(id);
            SliderItemDto sliderItemDto = new SliderItemDto()
            {
                Title = sliderItem.Title,
                ButtonUrl = sliderItem.ButtonUrl,
                ImageUrl = sliderItem.ImageUrl
            };
            return View(sliderItemDto);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, SliderItemDto sliderItemDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please, provide valid slider item details");
                return View(sliderItemDto);
            }

            var existingSliderItem = await _service.GetByIdAsync<SliderItem>(id);
            if (existingSliderItem == null)
            {
                return NotFound();
            }

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "upload", "sliderItems");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (sliderItemDto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingSliderItem.ImageUrl))
                {
                    string oldFilePath = Path.Combine(path, existingSliderItem.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderItemDto.ImageFile.FileName);
                string fullNewFilePath = Path.Combine(path, newFileName);

                using (FileStream fileStream = new FileStream(fullNewFilePath, FileMode.Create))
                {
                    sliderItemDto.ImageFile.CopyTo(fileStream);
                }

                sliderItemDto.ImageUrl = newFileName;
            }
            else
            {
                sliderItemDto.ImageUrl = existingSliderItem.ImageUrl;
            }

            existingSliderItem.Title = sliderItemDto.Title;
            existingSliderItem.ButtonUrl = sliderItemDto.ButtonUrl;
            existingSliderItem.ImageUrl = sliderItemDto.ImageUrl;

            await _service.UpdateAsync(existingSliderItem);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync<SliderItem>(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync<SliderItem>(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Restore(int id)
        {
            await _service.RestoreAsync<SliderItem>(id);
            return RedirectToAction(nameof(Trash));
        }
    }
}
