using Core.DTOs.BookDTOs;
using Core.DTOs.CategoryDTOs;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ICategoryRepository repository;

        public CategoryController(ApplicationDbContext context, ICategoryRepository repository)
        {
            this.context = context;
            this.repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categoriesFromDb = await repository.GetCategories();

            var categories = categoriesFromDb.Select(category => new CategoryResponseDTO
            {
                Name = category.Name,
                ID = category.CategoryId,
                Books = category.Books?.Select(book => new BookResponseDTO
                {
                    BookId = book.BookId,
                    Author = book.Author,
                    CategoryId = book.CategoryId,
                    AvaliableCopies = book.AvaliableCopies,
                    Description = book.Description,
                    Title = book.Title,
                    TotalCopies = book.TotalCopies,
                }).ToList() ?? new List<BookResponseDTO>()
            }).ToList();
            return Ok(categories);
        }

        [HttpGet("{CategoryId}")]
        public IActionResult GetCategoryById(int CategoryId)
        {
            if (CategoryId == 0)
                return BadRequest("InValid CategoryId");

            var categoryFromDb =  repository.GetById(CategoryId);
            if (categoryFromDb == null)
                return NotFound("There is no Category With this Id");

            var category = new CategoryResponseDTO
            {
                ID = categoryFromDb.CategoryId,
                Name = categoryFromDb.Name,
                Books = categoryFromDb.Books?.Select(book => new BookResponseDTO
                {
                    BookId = book.BookId,
                    Author = book.Author,
                    CategoryId = book.CategoryId,
                    AvaliableCopies = book.AvaliableCopies,
                    Description = book.Description,
                    Title = book.Title,
                    TotalCopies = book.TotalCopies,
                }).ToList() ?? new List<BookResponseDTO>()
            };

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await repository.Add(categoryDTO);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while Adding the category.");
            }
            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(recievedCategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryFromDb = await repository.FindCategory(categoryDTO.Id);
            if (categoryFromDb == null)
                return NotFound("There is no Category With this Id");

            categoryFromDb.Name = categoryDTO.Name;
            try
            {
                await repository.UpdateCategory(categoryFromDb);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the category.");
            }

            return NoContent();
        }

        [HttpDelete("{Categoryid}")]
        public async Task<IActionResult> DeleteCategory(int Categoryid)
        {
            var categoryFromDb = repository.GetById(Categoryid);
            if (categoryFromDb == null)
                return NotFound("There is no Category With this Id");

            if (categoryFromDb.Books?.Any() == true)
            {
                foreach (var book in categoryFromDb.Books)
                {
                    book.Category = null;
                    book.CategoryId = null;
                }
            }
            try
            {
                await repository.DeleteCategory(categoryFromDb);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while Deleting the category.");
            }
            return NoContent();
        }
    }
}
