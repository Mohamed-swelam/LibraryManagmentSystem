using Core.Entites;

namespace Core.Interfaces
{
    public interface IBorrowRepository
    {
        Task<int> MakeBorrow(string UserId,int BookId);
        Task<Borrowings?> GetBorrowById(int BorrowId);
        Task SaveChangesAsync();
        Task<List<Borrowings>?> GetBorrowings(string userId);
    }
}
