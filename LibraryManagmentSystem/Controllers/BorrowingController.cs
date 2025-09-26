using Core.DTOs.BorrowDTOs;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingController : ControllerBase
    {
        private readonly IBorrowRepository borrowRepository;
        private readonly IBookRepository bookRepository;

        public BorrowingController(IBorrowRepository borrowRepository, IBookRepository bookRepository)
        {
            this.borrowRepository = borrowRepository;
            this.bookRepository = bookRepository;
        }

        [Authorize]
        [HttpGet("{BorrowId}")]
        public async Task<IActionResult> GetBorrowithId(int BorrowId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var Borrow = await borrowRepository.GetBorrowById(BorrowId);
            if (Borrow == null)
                return NotFound("No Borrow With This Id..");

            var borrowResponse = new BorrowResponseDTO
            {
                BookId = Borrow.BookId,
                UserId = Borrow.UserId,
                Status = Borrow.Status,
                BorrowDate = Borrow.BorrowDate,
                DueDate = Borrow.DueDate,
                ReturnDate = Borrow.ReturnDate,
                Id = Borrow.Id
            };
            return Ok(borrowResponse);
        }

        [Authorize]
        [HttpGet("my-Borrowings")]
        public async Task<IActionResult> MyBorrowings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var BorrowingsFromDb = await borrowRepository.GetBorrowings(userId);

            var Borrowings = BorrowingsFromDb?.Select(Borrow => new BorrowResponseDTO
            {
                BookId= Borrow.BookId,
                UserId = Borrow.UserId,
                Status= Borrow.Status,
                BorrowDate= Borrow.BorrowDate,
                DueDate= Borrow.DueDate,
                Id= Borrow.Id,
                ReturnDate= Borrow.ReturnDate,
            }).ToList() ?? new List<BorrowResponseDTO>();

            return Ok(Borrowings);
        }



        [Authorize]
        [HttpPost("borrow-book/{BookId}")]
        public async Task<IActionResult> BorrowBook(int BookId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var bookFromDb = bookRepository.GetBookById(BookId);
            if (bookFromDb == null)
                return NotFound("No Book Wirh this Id..");

            int BorrowId;
            try
            {
                BorrowId = await borrowRepository.MakeBorrow(userId, BookId);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while Borrowing The Book..");
            }

            bookFromDb.AvaliableCopies--;
            await borrowRepository.SaveChangesAsync();

            var Borrow = await borrowRepository.GetBorrowById(BorrowId);

            var borrowResponse = new BorrowResponseDTO
            {
                BookId = Borrow!.BookId,
                UserId = Borrow.UserId,
                Status = Borrow.Status,
                BorrowDate = Borrow.BorrowDate,
                DueDate = Borrow.DueDate,
                ReturnDate = Borrow.ReturnDate,
                Id = Borrow.Id
            };

            return CreatedAtAction(nameof(GetBorrowithId), new { BorrowId }, borrowResponse);
        }

        

        [Authorize]
        [HttpPut("return-book/{BorrowId}")]
        public async Task<IActionResult> ReturnBook(int BorrowId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var Borrow = await borrowRepository.GetBorrowById(BorrowId);
            if (Borrow == null || Borrow.UserId != userId)
                return NotFound("No Borrow With This Id..");

            if (Borrow.Status == BorrowingStatus.returned || Borrow.Status == BorrowingStatus.overDue)
                return BadRequest("Book already returned.");


            Borrow.ReturnDate = DateTime.Now;
            if(Borrow.DueDate >= Borrow.ReturnDate)
                Borrow.Status = BorrowingStatus.returned;
            else
                Borrow.Status =BorrowingStatus.overDue;

            if (Borrow.Book != null)
                Borrow.Book.AvaliableCopies++;
            await borrowRepository.SaveChangesAsync();

            return Ok(new { message = "Book returned successfully" });
        }
    }
}
