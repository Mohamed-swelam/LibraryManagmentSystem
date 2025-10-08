using Core.DTOs.BookDTOs;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext context;

        public BookRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<int> Add(AddBookDTO book)
        {
            var newBook = new Book
            {
                Author = book.Author,
                Description = book.Description,
                Title = book.Title,
                TotalCopies = book.TotalCopies,
                CategoryId = book.CategoryId,
                AvaliableCopies = book.TotalCopies
            };
            await context.books.AddAsync(newBook);
            await context.SaveChangesAsync();

            return newBook.BookId;
        }

        public async Task DeleteBookAsync(Book book)
        {
            context.books.Remove(book);
            await context.SaveChangesAsync();
        }

        public async Task<Book?> FindBookAsync(int id)
        {
            return await context.books.FindAsync(id) ?? null;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await context.books.Include(e=>e.Borrowings)
                .Include(e=>e.Category).ToListAsync();
        }

        public IQueryable<Book> GetAsQueryable()
        {
           return  context.books.AsQueryable();
        }

        public Book? GetBookById(int id)
        {
            var book = context.books.Include(e => e.Category)
                .Include(e => e.Borrowings)
                .FirstOrDefault(e => e.BookId == id);
            return book ?? null;
        }

        public async Task saveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task UpdateBook(Book book)
        {
            context.books.Update(book);
            await context.SaveChangesAsync();
        }
    }
}
