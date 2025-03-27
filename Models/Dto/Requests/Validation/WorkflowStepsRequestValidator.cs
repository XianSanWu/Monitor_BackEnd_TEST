using FluentValidation;
using Utilities.ValidatorsUtil;

namespace Models.Dto.Requests.Validation
{
    public class WorkflowStepsRequestValidator
    {
        public class WorkflowStepsSearchListRequest : AbstractValidator<AuthRequest>
        {
            public WorkflowStepsSearchListRequest()
            {
               // RuleFor(x => x.UserName).NotEmpty().WithMessage("LoginRequest：使用者名稱不可空白");

               // RuleFor(x => x.Password)
               //.NotEmpty()
               //.WithMessage("LoginRequest：密碼不可空白")
               //.Must(value => RegExpUtil.IsMatch(RegExpUtil.RegexSymbolsEnglishNumbers, value))
               //.WithMessage("請輸入由英文、數字或符號組成的密碼");
            }
        }

        public class WorkflowStepsDetailRequest : AbstractValidator<AuthRequest>
        {
            public WorkflowStepsDetailRequest()
            {
                // RuleFor(x => x.UserName).NotEmpty().WithMessage("LoginRequest：使用者名稱不可空白");

                // RuleFor(x => x.Password)
                //.NotEmpty()
                //.WithMessage("LoginRequest：密碼不可空白")
                //.Must(value => RegExpUtil.IsMatch(RegExpUtil.RegexSymbolsEnglishNumbers, value))
                //.WithMessage("請輸入由英文、數字或符號組成的密碼");
            }
        }

    }
}
