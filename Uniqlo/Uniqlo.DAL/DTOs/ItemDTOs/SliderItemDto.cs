using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uniqlo.DAL.DTOs.ItemDTOs
{
    public class SliderItemDto 
    {
        public string Title { get; set; }
        public string ButtonUrl { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
