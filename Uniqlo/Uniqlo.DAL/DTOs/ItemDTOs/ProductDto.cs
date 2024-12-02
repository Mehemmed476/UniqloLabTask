using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Uniqlo.DAL.Models;

namespace Uniqlo.DAL.DTOs.ItemDTOs
{
    public class ProductDto
    {
        public string Title { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public string? ImageUrl { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
