using Core.DTOs.BookDTOs;
using Core.DTOs.GeneralDTOs;
using Core.Entites;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Text.Json;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository bookRepository;
        private readonly ICategoryRepository categoryRepository;
        private const int MaxPageSize = 50;
        public BookController(IBookRepository bookRepository, ICategoryRepository categoryRepository)
        {
            this.bookRepository = bookRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery]GetBooksDTO getBooksDTO)
        {
            var books = bookRepository.GetAsQueryable();
            if (getBooksDTO.testFilter != null)
            {
                List<FilterCondition>? filters = JsonConvert.DeserializeObject<List<FilterCondition>>(getBooksDTO.testFilter);
                books = FilterBuilder.ApplyFilter(books, filters);
            }
            var totalCount = await books.CountAsync();
            var pageSize = getBooksDTO.PageSize > MaxPageSize ? MaxPageSize : getBooksDTO.PageSize;

            var PaginatedBooks = books
                                            .Skip((getBooksDTO.PageNumber - 1) * getBooksDTO.PageSize)
                                            .Take(getBooksDTO.PageSize).ToList();


            var Books = PaginatedBooks.Select(book => new BookResponseDTO
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

            return Ok(new PagedResult<BookResponseDTO>(Books, totalCount, getBooksDTO.PageNumber, pageSize));
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

        [HttpPut("update-book/{bookId}")]
        public async Task<IActionResult> UpdateBook(int bookId, [FromBody] JsonElement data)
        {
            var BookFromDb = await bookRepository.FindBookAsync(bookId);
            if (BookFromDb == null)
                return NotFound("There is no Book with this Id");

            var newDat = data.GetRawText();
            JsonConvert.PopulateObject(newDat, BookFromDb);

            //var jsonDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(data.GetRawText());
            //foreach (var prop in jsonDict)
            //{
            //    var propertyInfo = BookFromDb.GetType()
            //        .GetProperty(prop.Key, System.Reflection.BindingFlags.IgnoreCase 
            //        | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            //    if (propertyInfo != null)
            //    {
            //        var convertedValue = Convert.ChangeType(prop.Value, propertyInfo.PropertyType);
            //        propertyInfo.SetValue(BookFromDb, convertedValue);
            //    }
            //}
            try
            {
                await bookRepository.saveChangesAsync();
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
