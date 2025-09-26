using Core.Entites;
using Core.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly ApplicationDbContext context;

        public BorrowRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Borrowings?> GetBorrowById(int BorrowId)
        {
            return await context.borrowings.Include(e => e.User)
                .Include(e => e.Book).FirstOrDefaultAsync(e => e.Id == BorrowId)
                ?? null;
        }

        public async Task<List<Borrowings>?> GetBorrowings(string userId)
        {
            return await context.borrowings.Include(e => e.Book).Include(e => e.User)
                .Where(e => e.UserId == userId).ToListAsync() ?? new List<Borrowings>();
        }

        public async Task<int> MakeBorrow(string UserId, int BookId)
        {
            var Borrow = new Borrowings
            {
                BookId = BookId,
                UserId = UserId,
                Status = BorrowingStatus.Borrowed,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };

            await context.borrowings.AddAsync(Borrow);
            await SaveChangesAsync();
            return Borrow.Id;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
