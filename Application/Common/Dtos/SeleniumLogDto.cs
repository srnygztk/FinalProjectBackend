using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos
{
    public class SeleniumLogDto
    {
        public string Message { get; set; }
        public DateTimeOffset SentOn { get; set; }
        public OrderStatus Status { get; set; }

        public SeleniumLogDto(string message, OrderStatus status)
        {
            Message = message;
            Status = status;
            SentOn = DateTimeOffset.Now;
        }
    }
}