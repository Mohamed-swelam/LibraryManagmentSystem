using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public BorrowingController(ApplicationDbContext context)
        {
            this.context = context;
        }


    }
}
