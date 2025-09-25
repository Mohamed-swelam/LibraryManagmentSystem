using Core.DTOs.BookDTOs;
using Core.Entites;

namespace Core.Interfaces
{
    public interface IBookRepository
    {
        Task<int> Add(AddBookDTO book);
        Book? GetBookById(int id);
    }
}
