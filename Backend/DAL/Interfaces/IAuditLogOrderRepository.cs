using Backend.DAL.Models;

namespace Backend.DAL.Interfaces
{
    public interface IAuditLogOrderRepository
    {
        Task<V1AuditLogOrderDal[]> BulkInsert(V1AuditLogOrderDal[] auditLogs, CancellationToken token);
        Task<V1AuditLogOrderDal[]> Query(QueryAuditLogOrderDalModel model, CancellationToken token);
    }
    public class QueryAuditLogOrderDalModel
    {
        public long[] Ids { get; set; }
        public long[] OrderIds { get; set; }
        public long[] CustomerIds { get; set; }
        public string[] OrderStatuses { get; set; }
        public int Limit { get; set; } = 100;
        public int Offset { get; set; }
    }
}