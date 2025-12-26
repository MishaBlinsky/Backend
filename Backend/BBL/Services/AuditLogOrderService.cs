using Backend.BBL.Models;
using Backend.DAL.Interfaces;
using Backend.DAL.Models;

namespace Backend.BBL.Services
{
    public class AuditLogOrderService
    {
        private readonly IAuditLogOrderRepository _auditLogOrderRepository;
        public AuditLogOrderService(IAuditLogOrderRepository auditLogOrderRepository)
        {
            _auditLogOrderRepository = auditLogOrderRepository;
        }
        public async Task<V1AuditLogOrderResponse> CreateAuditLogs(V1AuditLogOrderRequest request, CancellationToken token)
        {
            var now = DateTimeOffset.UtcNow;
            var auditLogsDal = request.Orders.Select(order => new V1AuditLogOrderDal
            {
                OrderId = order.OrderId,
                OrderItemId = order.OrderItemId,
                CustomerId = order.CustomerId,
                OrderStatus = order.OrderStatus,
                CreatedAt = now,
                UpdatedAt = now
            }).ToArray();
            var savedLogs = await _auditLogOrderRepository.BulkInsert(auditLogsDal, token);
            return new V1AuditLogOrderResponse
            {
                AuditLogs = savedLogs.Select(log => new V1AuditLogOrderResponse.AuditLogOrder
                {
                    Id = log.Id,
                    OrderId = log.OrderId,
                    OrderItemId = log.OrderItemId,
                    CustomerId = log.CustomerId,
                    OrderStatus = log.OrderStatus,
                    CreatedAt = log.CreatedAt,
                    UpdatedAt = log.UpdatedAt
                }).ToArray()
            };
        }
        public async Task<V1AuditLogOrderResponse> GetAuditLogs(QueryAuditLogOrderModel model, CancellationToken token)
        {
            var dalModel = new QueryAuditLogOrderDalModel
            {
                Ids = model.Ids,
                OrderIds = model.OrderIds,
                CustomerIds = model.CustomerIds,
                OrderStatuses = model.OrderStatuses,
                Limit = model.PageSize,
                Offset = (model.Page - 1) * model.PageSize
            };
            var logs = await _auditLogOrderRepository.Query(dalModel, token);
            return new V1AuditLogOrderResponse
            {
                AuditLogs = logs.Select(log => new V1AuditLogOrderResponse.AuditLogOrder
                {
                    Id = log.Id,
                    OrderId = log.OrderId,
                    OrderItemId = log.OrderItemId,
                    CustomerId = log.CustomerId,
                    OrderStatus = log.OrderStatus,
                    CreatedAt = log.CreatedAt,
                    UpdatedAt = log.UpdatedAt
                }).ToArray()
            };
        }
    }
    public class QueryAuditLogOrderModel
    {
        public long[] Ids { get; set; }
        public long[] OrderIds { get; set; }
        public long[] CustomerIds { get; set; }
        public string[] OrderStatuses { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}