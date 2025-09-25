using Core.DTOs.BookDTOs;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository bookRepository;
        private readonly ApplicationDbContext context;

        public BookController(IBookRepository bookRepository, ApplicationDbContext context)
        {
            this.bookRepository = bookRepository;
            this.context = context;
        }


        [HttpPost("{bookId}")]
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


        
    }
}
