using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniqlo.DAL.Models.Base;

namespace Uniqlo.DAL.Models
{
    public class Product : BaseAuditableEntity
    {
        public string Title { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
