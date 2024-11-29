using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniqlo.BL.Services.Abstractions;
using Uniqlo.DAL.Contexts;
using Uniqlo.DAL.Models.Base;

namespace Uniqlo.BL.Services.Concretes
{
    public class GenericCRUDService : IGenericCRUDService
    {
        private readonly UniqloDbContext _context;

        public GenericCRUDService(UniqloDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            IEnumerable<T>? entities = await _context.Set<T>().ToListAsync();
            return entities;
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new NullReferenceException("No data found.");
            }
            return entity;
        }

        public async Task CreateAsync<T>(T entity) where T : BaseAuditableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
            }

            entity.CreatedDate = DateTime.Now;

            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T entity) where T : BaseAuditableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
            }

            var existingEntity = await _context.Set<T>().FindAsync(entity.Id);

            if (existingEntity == null)
            {
                throw new ArgumentException($"{typeof(T).Name} not found.");
            }

            entity.ModifiedDate = DateTime.Now;

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(int id) where T : BaseAuditableEntity
        {
            var existingEntity = await _context.Set<T>().FindAsync(id);

            if (existingEntity == null)
            {
                throw new ArgumentException($"{typeof(T).Name} not found.");
            }

            _context.Set<T>().Remove(existingEntity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync<T>(int id) where T : BaseAuditableEntity
        {
            var existingEntity = await _context.Set<T>().FindAsync(id);

            if (existingEntity == null)
            {
                throw new ArgumentException($"{typeof(T).Name} not found.");
            }

            existingEntity.DeletedDate = DateTime.Now;
            existingEntity.IsDeleted = true;
            _context.Set<T>().Update(existingEntity);
            await _context.SaveChangesAsync();

        }

        public async Task RestoreAsync<T>(int id) where T : BaseAuditableEntity
        {
            var existingEntity = await _context.Set<T>().FindAsync(id);

            if (existingEntity == null)
            {
                throw new ArgumentException($"{typeof(T).Name} not found.");
            }

            existingEntity.IsDeleted = false;
            existingEntity.DeletedDate = null;
            _context.Set<T>().Update(existingEntity);
            await _context.SaveChangesAsync();
        }
    }
}
