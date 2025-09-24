using Core.DTOs.CategoryDTOs;
using Core.Entites;

namespace Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task Add(AddCategoryDTO categoryDTO);
        Task<List<Category>> GetCategories();
        Category? GetById (int id);
        Task<Category?> FindCategory(int id);
        Task UpdateCategory(Category category);

        Task DeleteCategory(Category category);
    }
}
