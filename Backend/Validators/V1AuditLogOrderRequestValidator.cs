using FluentValidation;
using Backend.DAL.Models;

namespace Backend.Validators
{
    public class V1AuditLogOrderRequestValidator : AbstractValidator<V1AuditLogOrderRequest>
    {
        public V1AuditLogOrderRequestValidator()
        {
            RuleFor(x => x.Orders)
                .NotEmpty().WithMessage("Orders cannot be empty")
                .Must(orders => orders.Length <= 1000).WithMessage("Cannot process more than 1000 orders at once");
            RuleForEach(x => x.Orders).ChildRules(order =>
            {
                order.RuleFor(o => o.OrderId).GreaterThan(0).WithMessage("OrderId must be positive");
                order.RuleFor(o => o.OrderItemId).GreaterThan(0).WithMessage("OrderItemId must be positive");
                order.RuleFor(o => o.CustomerId).GreaterThan(0).WithMessage("CustomerId must be positive");
                order.RuleFor(o => o.OrderStatus).NotEmpty().MaximumLength(50).WithMessage("OrderStatus is required and max length is 50");
            });
        }
    }
}