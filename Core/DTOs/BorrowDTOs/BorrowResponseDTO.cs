using Core.Helpers;
using System.Text.Json.Serialization;

namespace Core.DTOs.BorrowDTOs
{
    public class BorrowResponseDTO
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BorrowingStatus Status { get; set; }
    }
}
