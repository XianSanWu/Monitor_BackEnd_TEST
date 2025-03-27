using FluentValidation;
using Utilities.ValidatorsUtil;
using static Models.Dto.Requests.WorkflowStepsRequest;

namespace Models.Dto.Requests.Validation
{
    public class WorkflowStepsRequestValidator
    {
        public class WorkflowStepsSearchListRequestValidator : AbstractValidator<WorkflowStepsSearchListRequest>
        {
            public WorkflowStepsSearchListRequestValidator()
            {
               // RuleFor(x => x.UserName).NotEmpty().WithMessage("LoginRequest：使用者名稱不可空白");

               // RuleFor(x => x.Password)
               //.NotEmpty()
               //.WithMessage("LoginRequest：密碼不可空白")
               //.Must(value => RegExpUtil.IsMatch(RegExpUtil.RegexSymbolsEnglishNumbers, value))
               //.WithMessage("請輸入由英文、數字或符號組成的密碼");
            }
        }

        public class WorkflowStepsDetailRequestValidator : AbstractValidator<AuthRequest>
        {
            public WorkflowStepsDetailRequestValidator()
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
