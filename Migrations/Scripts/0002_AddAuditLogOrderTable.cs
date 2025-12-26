using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrations.Scripts
{
    [Migration(2)]
    public class AddAuditLogOrderTable : Migration
    {
        public override void Up()
        {
            var sql = @"
            create table if not exists audit_log_order (
                id bigserial not null primary key,
                order_id bigint not null,
                order_item_id bigint not null,
                customer_id bigint not null,
                order_status text not null,
                created_at timestamp with time zone not null,
                updated_at timestamp with time zone not null
            );
            create index if not exists idx_audit_log_order_id on audit_log_order (order_id);
            create index if not exists idx_audit_log_customer_id on audit_log_order (customer_id);
            create index if not exists idx_audit_log_order_status on audit_log_order (order_status);
            create type v1_audit_log_order as (
                id bigint,
                order_id bigint,
                order_item_id bigint,
                customer_id bigint,
                order_status text,
                created_at timestamp with time zone,
                updated_at timestamp with time zone
            );
        ";
            Execute.Sql(sql);
        }
        public override void Down()
        {
            var sql = @"
            drop table if exists audit_log_order;
            drop type if exists v1_audit_log_order;
        ";
            Execute.Sql(sql);
        }
    }
}