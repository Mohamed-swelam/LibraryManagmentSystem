using Core.DTOs.BookDTOs;
using Core.Entites;

namespace Core.Interfaces
{
    public interface IBookRepository
    {
        Task<int> Add(AddBookDTO book);
        Book? GetBookById(int id);
        Task<List<Book>> GetAllBooks();
        Task<Book?> FindBookAsync(int id);
        Task UpdateBook(Book book);
        Task DeleteBookAsync(Book book);
    }
}
