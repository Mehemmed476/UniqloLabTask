using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniqlo.DAL.Models.Base;

namespace Uniqlo.DAL.Models
{
    public class Category : BaseAuditableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
