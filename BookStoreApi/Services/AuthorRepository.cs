using BookStoreApi.Contracts;
using BookStoreApi.Data;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _db;
        public AuthorRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Author entity)
        {
            await _db.Authors.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Author entity)
        {
            _db.Authors.Remove(entity);
            return await Save();
        }

        public async Task<IList<Author>> FindAll()
        {
            return await _db.Authors.ToListAsync();
        }

        public async Task<Author> FindById(int id)
        {
            return await _db.Authors.FindAsync(id);
        }

        public async Task<bool> IsExists(int id)
        {
            return await _db.Authors.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Author entity)
        {
            _db.Authors.Update(entity);
            return await Save();
        }
    }
}
