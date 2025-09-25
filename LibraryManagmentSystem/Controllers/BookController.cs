using Core.DTOs.BookDTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository bookRepository;
        private readonly ICategoryRepository categoryRepository;

        public BookController(IBookRepository bookRepository, ICategoryRepository categoryRepository)
        {
            this.bookRepository = bookRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var BooksFromDb = await bookRepository.GetAllBooks();

            var Books = BooksFromDb.Select(book => new BookResponseDTO
            {
                BookId = book.BookId,
                Author = book.Author,
                AvaliableCopies = book.AvaliableCopies,
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name ?? "No Category",
                Description = book.Description,
                Title = book.Title,
                TotalCopies = book.TotalCopies,
            }).ToList();
            return Ok(Books);
        }


        [HttpGet("{bookId}")]
        public IActionResult GetBookById(int bookId)
        {
            if (bookId <= 0)
                return BadRequest("InValid Id");

            var book = bookRepository.GetBookById(bookId);
            if (book == null)
                return NotFound("There is no Book with this Id");

            var bookresponse = new BookResponseDTO
            {
                BookId = bookId,
                Author = book.Author,
                Description = book.Description,
                CategoryId = book.CategoryId,
                AvaliableCopies = book.TotalCopies,
                Title = book.Title,
                TotalCopies = book.TotalCopies,
                CategoryName = book.Category?.Name ?? "No Category"
            };
            return Ok(bookresponse);
        }


        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookDTO bookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int BookId;
            try
            {
                BookId = await bookRepository.Add(bookDTO);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while Adding the book.");
            }

            var bookResponse = new BookResponseDTO
            {
                BookId = BookId,
                Author = bookDTO.Author,
                Description = bookDTO.Description,
                Title = bookDTO.Title,
                TotalCopies = bookDTO.TotalCopies,
                CategoryId = bookDTO.CategoryId,
                AvaliableCopies = bookDTO.TotalCopies
            };

            return CreatedAtAction(nameof(GetBookById), new { bookId = BookId }, bookResponse);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(recievedBookDTO bookDTO)
        {
            var BookFromDb = await bookRepository.FindBookAsync(bookDTO.BookId);
            if (BookFromDb == null)
                return NotFound("There is no Book with this Id");

            if (bookDTO.CategoryId is not null)
            {
                var category = await categoryRepository.FindCategory(bookDTO.CategoryId.Value);
                if (category == null)
                    return NotFound("There is no Category With this id");
            }

            BookFromDb.Title = bookDTO.Title;
            BookFromDb.Author = bookDTO.Author;
            BookFromDb.AvaliableCopies = bookDTO.TotalCopies;
            BookFromDb.CategoryId = bookDTO.CategoryId;
            BookFromDb.Description = bookDTO.Description;
            BookFromDb.TotalCopies = bookDTO.TotalCopies;

            try
            {
                await bookRepository.UpdateBook(BookFromDb);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while Updating the book.");
            }
            return NoContent();
        }


        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteBook(int BookId)
        {
            var BookFromDb = await bookRepository.FindBookAsync(BookId);
            if (BookFromDb == null)
                return NotFound("There is no Book with this Id");

            try
            {
                await bookRepository.DeleteBookAsync(BookFromDb);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while Deleting the book.");
            }
            return NoContent();
        }

    }
}
