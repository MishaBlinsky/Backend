using Dapper;
using Backend.DAL.Interfaces;
using Backend.DAL.Models;
using System.Text;

namespace Backend.DAL.Repositories
{
    public class OrderItemRepository(UnitOfWork unitOfWork) : IOrderItemRepository
    {
        public async Task<V1OrderItemDal[]> BulkInsert(V1OrderItemDal[] models, CancellationToken token)
        {
            var sql = @"
            INSERT INTO order_items 
            (
                order_id,
                product_id,
                quantity,
                product_title,
                product_url,
                price_cents,
                price_currency,
                created_at,
                updated_at
            )
            SELECT 
                order_id,
                product_id,
                quantity,
                product_title,
                product_url,
                price_cents,
                price_currency,
                created_at,
                updated_at
            FROM unnest(@OrderItems)
            RETURNING 
                id,
                order_id,
                product_id,
                quantity,
                product_title,
                product_url,
                price_cents,
                price_currency,
                created_at,
                updated_at;
        ";
            var conn = await unitOfWork.GetConnection(token);
            var res = await conn.QueryAsync<V1OrderItemDal>(new CommandDefinition(
                sql, new { OrderItems = models }, cancellationToken: token));
            return res.ToArray();
        }
        public async Task<V1OrderItemDal[]> Query(QueryOrderItemsDalModel model, CancellationToken token)
        {
            var sql = new StringBuilder(@"
            SELECT 
                id,
                order_id,
                product_id,
                quantity,
                product_title,
                product_url,
                price_cents,
                price_currency,
                created_at,
                updated_at
            FROM order_items
        ");
            var param = new DynamicParameters();
            var conditions = new List<string>();
            // Фильтр по ID элементов заказа
            if (model.Ids?.Length > 0)
            {
                param.Add("Ids", model.Ids);
                conditions.Add("id = ANY(@Ids)");
            }
            // Фильтр по ID заказов
            if (model.OrderIds?.Length > 0)
            {
                param.Add("OrderIds", model.OrderIds);
                conditions.Add("order_id = ANY(@OrderIds)");
            }
            // Добавляем условия WHERE если они есть
            if (conditions.Count > 0)
            {
                sql.Append(" WHERE " + string.Join(" AND ", conditions));
            }
            // Пагинация
            if (model.Limit > 0)
            {
                sql.Append(" LIMIT @Limit");
                param.Add("Limit", model.Limit);
            }
            if (model.Offset > 0)
            {
                sql.Append(" OFFSET @Offset");
                param.Add("Offset", model.Offset);
            }
            var conn = await unitOfWork.GetConnection(token);
            var res = await conn.QueryAsync<V1OrderItemDal>(new CommandDefinition(
                sql.ToString(), param, cancellationToken: token));
            return res.ToArray();
        }
    }
}