using Microsoft.AspNetCore.Mvc;
using Uniqlo.BL.Services.Abstractions;
using Uniqlo.DAL.DTOs.ItemDTOs;
using Uniqlo.DAL.Models;

namespace Uniqlo.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IGenericCRUDService _service;
        IWebHostEnvironment _webHostEnvironment;

        public CategoryController(IGenericCRUDService service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _service.GetAllAsync<Category>();
            List<Category> activeCategorys = categories.Where(s => !s.IsDeleted).ToList();
            return View(activeCategorys);
        }

        [HttpGet]

        public async Task<IActionResult> Trash()
        {
            IEnumerable<Category> categories = await _service.GetAllAsync<Category>();
            List<Category> deactiveCategorys = categories.Where(s => s.IsDeleted).ToList();
            return View(deactiveCategorys);
        }

        public async Task<IActionResult> Details(int id)
        {
            Category category = await _service.GetByIdAsync<Category>(id);
            return View(category);
        }
        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please, choose a valid slider item");
                return View(categoryDto);
            }

            Category category = new Category()
            {
                Title = categoryDto.Title,
                Description = categoryDto.Description
            };

            await _service.CreateAsync(category);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            Category category = await _service.GetByIdAsync<Category>(id);
            CategoryDto categoryDto = new CategoryDto()
            {
                Title = category.Title,
                Description = category.Description
            };
            return View(categoryDto);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please, provide valid slider item details");
                return View(categoryDto);
            }

            var existingCategory = await _service.GetByIdAsync<Category>(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Title = categoryDto.Title;
            existingCategory.Description = categoryDto.Description;

            await _service.UpdateAsync(existingCategory);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync<Category>(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync<Category>(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Restore(int id)
        {
            await _service.RestoreAsync<Category>(id);
            return RedirectToAction(nameof(Trash));
        }
    }
}
