using FluentValidation;
using static Models.Dto.Requests.MailHunterRequest;
namespace Models.Dto.Requests.Validation
{
    public class MailHunterRequestValidator
    {
        public class MailHunterSearchListRequestValidator : AbstractValidator<MailHunterSearchListRequest>
        {
            public MailHunterSearchListRequestValidator()
            {
                RuleFor(x => x.FieldModel).NotNull().WithMessage("FieldModel 不可為 null");

#pragma warning disable CS8602 // 可能 null 參考的取值 (dereference)。
                RuleFor(x => x.FieldModel.Department).NotNull().NotEmpty().WithMessage("部門不可空白");
                RuleFor(x => x.FieldModel.StartDate)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("起始日期不可空白")
                    .Must(date => date > DateTime.MinValue).WithMessage("請選擇有效的起始日期");

                RuleFor(x => x.FieldModel.EndDate)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("結束日期不可空白")
                    .Must(date => date > DateTime.MinValue).WithMessage("請選擇有效的結束日期");

                RuleFor(x => x.FieldModel)
                    .Must(x => x.StartDate <= x.EndDate)
                    .WithMessage("起始日期不能晚於結束日期");


                //RuleFor(x => x.FieldModel.StartDate)
                //    .GreaterThan(DateTime.Now)
                //    .WithMessage("起始日期必須是未來時間");

#pragma warning restore CS8602 // 可能 null 參考的取值 (dereference)。
            }
        }

    }
}
