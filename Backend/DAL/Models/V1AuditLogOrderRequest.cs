namespace Backend.DAL.Models
{
    public class V1AuditLogOrderRequest
    {
        public LogOrder[] Orders { get; set; }
        public class LogOrder
        {
            public long OrderId { get; set; }
            public long OrderItemId { get; set; }
            public long CustomerId { get; set; }
            public string OrderStatus { get; set; }
        }
    }
}