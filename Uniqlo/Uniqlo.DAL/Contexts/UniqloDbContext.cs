using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniqlo.DAL.Models;

namespace Uniqlo.DAL.Contexts;

public class UniqloDbContext : DbContext
{
    public UniqloDbContext(DbContextOptions<UniqloDbContext> options) : base(options) { }
    public DbSet<SliderItem> SliderItems { get; set; }
}
