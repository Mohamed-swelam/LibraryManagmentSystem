using Core.DTOs.CategoryDTOs;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task Add(AddCategoryDTO categoryDTO)
        {
            var category = new Category { Name = categoryDTO.Name };
            await context.AddAsync(category);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCategory(Category category)
        {
            context.categories.Remove(category);
            await context.SaveChangesAsync();
        }

        public async Task<Category?> FindCategory(int id)
        {
            var category = await context.categories.FindAsync(id);
            return category ?? null;
        }

        public Category? GetById(int id)
        {
            var Category= context.categories.Include("Books").FirstOrDefault(e=>e.CategoryId == id);
            return Category ?? null;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await context.categories.Include("Books").ToListAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            context.categories.Update(category);
            await context.SaveChangesAsync();
        }
    }
}
