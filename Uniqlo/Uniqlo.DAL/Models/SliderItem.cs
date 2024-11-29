using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniqlo.DAL.Models.Base;

namespace Uniqlo.DAL.Models;

public class SliderItem : BaseAuditableEntity
{
    public string Title { get; set; }
    public string ButtonUrl { get; set; }
    public string ImageUrl { get; set; }
}
