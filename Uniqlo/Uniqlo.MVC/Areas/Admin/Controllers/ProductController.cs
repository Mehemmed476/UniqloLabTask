using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Uniqlo.BL.Services.Abstractions;
using Uniqlo.DAL.DTOs.ItemDTOs;
using Uniqlo.DAL.Models;

namespace Uniqlo.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IGenericCRUDService _service;
        IWebHostEnvironment _webHostEnvironment;

        public ProductController(IGenericCRUDService service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _service.GetAllAsync<Product>();
            List<Product> activeProducts = products.Where(s => !s.IsDeleted).ToList();
            return View(activeProducts);
        }

        [HttpGet]

        public async Task<IActionResult> Trash()
        {
            IEnumerable<Product> products = await _service.GetAllAsync<Product>();
            List<Product> deactiveProducts = products.Where(s => s.IsDeleted).ToList();
            return View(deactiveProducts);
        }

        public async Task<IActionResult> Details(int id)
        {
            Product product = await _service.GetByIdAsync<Product>(id);
            return View(product);
        }
        [HttpGet]

        public async Task<IActionResult> Create()
        {
            var model = new ProductDto()
            {
                Categories = Task.Run(() => _service.GetAllAsync<Category>()).Result.Where(d => !d.IsDeleted).Select(x => new System.Web.Mvc.SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                }),
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                var model = new ProductDto()
                {
                    Categories = Task.Run(() => _service.GetAllAsync<Category>()).Result.Where(d => !d.IsDeleted).Select(x => new System.Web.Mvc.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    }),
                    Title = productDto.Title,
                    Image = productDto.Image,
                    ImageUrl = productDto.ImageUrl,
                    Price = productDto.Price,
                    CategoryId = productDto.CategoryId
                };
                ModelState.AddModelError(string.Empty, "Please, choose a valid slider item");
                return View(model);
            }

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "upload", "products");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
            string fullPath = Path.Combine(path, fileName);

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                productDto.Image.CopyTo(fileStream);
            }

            productDto.ImageUrl = fileName;

            Product product = new Product()
            {
                Title = productDto.Title,
                Image = productDto.Image,
                ImageUrl = productDto.ImageUrl,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId

            };

            await _service.CreateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            Product product = await _service.GetByIdAsync<Product>(id);
            IEnumerable<Category> categories = await _service.GetAllAsync<Category>();
            ProductDto productDto = new ProductDto()
            {
                Categories = Task.Run(() => _service.GetAllAsync<Category>()).Result.Where(d => !d.IsDeleted).Select(x => new System.Web.Mvc.SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                }),
                Title = product.Title,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
            return View(productDto);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, ProductDto productDto)
        {
            IEnumerable<Category> categories = await _service.GetAllAsync<Category>();

            if (!ModelState.IsValid)
            {
                ProductDto exitingProductDto = new ProductDto()
                {
                    Categories = Task.Run(() => _service.GetAllAsync<Category>()).Result.Where(d => !d.IsDeleted).Select(x => new System.Web.Mvc.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    }),
                    Title = productDto.Title,
                    ImageUrl = productDto.ImageUrl,
                    Price = productDto.Price,
                    CategoryId = productDto.CategoryId
                };
                ModelState.AddModelError(string.Empty, "Please, provide valid slider item details");
                return View(exitingProductDto);
            }

            var existingProduct = await _service.GetByIdAsync<Product>(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "upload", "products");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (productDto.Image != null)
            {
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    string oldFilePath = Path.Combine(path, existingProduct.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
                string fullNewFilePath = Path.Combine(path, newFileName);

                using (FileStream fileStream = new FileStream(fullNewFilePath, FileMode.Create))
                {
                    productDto.Image.CopyTo(fileStream);
                }

                productDto.ImageUrl = newFileName;
            }
            else
            {
                productDto.ImageUrl = existingProduct.ImageUrl;
            }

            existingProduct.Title = productDto.Title;
            existingProduct.Title = productDto.Title;
            existingProduct.ImageUrl = productDto.ImageUrl;
            existingProduct.Price = productDto.Price;
            existingProduct.CategoryId = productDto.CategoryId;

            await _service.UpdateAsync(existingProduct);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync<Product>(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync<Product>(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Restore(int id)
        {
            await _service.RestoreAsync<Product>(id);
            return RedirectToAction(nameof(Trash));
        }
    }
}
