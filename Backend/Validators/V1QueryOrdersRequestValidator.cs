using FluentValidation;
using Backend.BBL.Models;
using Models.Dto.V1.Requests;

namespace Backend.Validators
{
    public class V1QueryOrdersRequestValidator : AbstractValidator<V1QueryOrdersRequest>
    {
        public V1QueryOrdersRequestValidator()
        {
            RuleFor(x => x).Must(x => (x.Ids?.Length>0) ||(x.CustomerIds?.Length>0)).WithMessage("CustomerId or Ids must be in querry");
            // Валидация для Page
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Page must be greater than or equal to 0");
            // Валидация для PageSize
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage("PageSize must be greater than or equal to 0")
                .LessThanOrEqualTo(1000)
                .WithMessage("PageSize cannot exceed 1000");
            // Валидация для Ids (если указаны)
            When(x => x.Ids != null && x.Ids.Length > 0, () =>
            {
                RuleFor(x => x.Ids)
                    .Must(ids => ids.All(id => id > 0))
                    .WithMessage("All IDs must be greater than 0")
                    .Must(ids => ids.Length <= 1000)
                    .WithMessage("Cannot query more than 1000 IDs at once");
            });
            // Валидация для CustomerIds (если указаны)
            When(x => x.CustomerIds != null && x.CustomerIds.Length > 0, () =>
            {
                RuleFor(x => x.CustomerIds)
                    .Must(ids => ids.All(id => id > 0))
                    .WithMessage("All Customer IDs must be greater than 0")
                    .Must(ids => ids.Length <= 1000)
                    .WithMessage("Cannot query more than 1000 Customer IDs at once");
            });
            // Валидация комбинации Page и PageSize
            When(x => x.Page > 0, () =>
            {
                RuleFor(x => x.PageSize)
                    .GreaterThan(0)
                    .WithMessage("PageSize must be greater than 0 when Page is specified");
            });
            When(x => x.PageSize > 0, () =>
            {
                RuleFor(x => x.Page)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Page must be specified when PageSize is greater than 0");
            });
        }
    }
}