using FluentValidation;
using Models.Dto.V1.Requests;

namespace Backend.Validators
{
    public class V1CreateOrderRequestValidator : AbstractValidator<V1CreateOrderRequest>
    {
        public V1CreateOrderRequestValidator()
        {
            // правило того, что заказы не могут быть null или пустыми
            RuleFor(x => x.Orders).NotEmpty();
            // правило для каждого заказа в массиве вызови OrderValidator
            RuleForEach(x => x.Orders)
                .SetValidator(new OrderValidator())
                .When(x => x.Orders is not null);
        }
        public class OrderValidator : AbstractValidator<V1CreateOrderRequest.Order>
        {
            public OrderValidator()
            {
                // CustomerId в каждом заказе не должен быть меньше или равен 0
                RuleFor(x => x.CustomerId).GreaterThan(0);
                // Не пустая строка в DeliveryAddress
                RuleFor(x => x.DeliveryAddress).NotEmpty();
                // TotalPriceCents в каждом заказе не должен быть меньше или равен 0
                RuleFor(x => x.TotalPriceCents).GreaterThan(0);
                RuleFor(x => x.TotalPriceCurrency).NotEmpty();
                // OrderItems в каждом заказе не должен быть null или пустым
                RuleFor(x => x.OrderItems).NotEmpty();
                // Для каждого OrderItem вызови OrderItemValidator
                RuleForEach(x => x.OrderItems)
                    .SetValidator(new OrderItemValidator())
                    .When(x => x.OrderItems is not null);
                // TotalPriceCents в каждом заказе должен быть равен сумме всех OrderItems.PriceCents * OrderItems.Quantity
                RuleFor(x => x)
                    .Must(x => x.TotalPriceCents == x.OrderItems.Sum(y => y.PriceCents * y.Quantity))
                    .When(x => x.OrderItems is not null)
                    .WithMessage("TotalPriceCents should be equal to sum of all OrderItems.PriceCents * OrderItems.Quantity");
                // Все PriceCurrency в OrderItems должны быть одинаковы
                RuleFor(x => x)
                    .Must(x => x.OrderItems.Select(r => r.PriceCurrency).Distinct().Count() == 1)
                    .When(x => x.OrderItems is not null)
                    .WithMessage("All OrderItems.PriceCurrency should be the same");
                // OrderItems.PriceCurrency хотя бы в первом OrderItem должен быть равен TotalPriceCurrency
                // Если во втором не равен, то предыдущее правило выкинуло ошибку
                RuleFor(x => x)
                    .Must(x => x.OrderItems.Select(r => r.PriceCurrency).First() == x.TotalPriceCurrency)
                    .When(x => x.OrderItems is not null)
                    .WithMessage("OrderItems.PriceCurrency should be the same as TotalPriceCurrency");
            }
        }
        // тут все просто
        public class OrderItemValidator : AbstractValidator<V1CreateOrderRequest.OrderItem>
        {
            public OrderItemValidator()
            {
                RuleFor(x => x.ProductId).GreaterThan(0);
                RuleFor(x => x.PriceCents).GreaterThan(0);
                RuleFor(x => x.PriceCurrency).NotEmpty();
                RuleFor(x => x.ProductTitle).NotEmpty();
                RuleFor(x => x.Quantity).GreaterThan(0);
            }
        }
    }
}