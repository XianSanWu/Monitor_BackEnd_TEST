using FluentValidation;
using Utilities.ValidatorsUtil;
using static Models.Dto.Requests.AuthRequest;

namespace Models.Dto.Requests.Validation
{
    public class AuthRequestValidator
    {
        public class LoginRequestValidator : AbstractValidator<LoginRequest>
        {
            public LoginRequestValidator()
            {
                RuleFor(x => x.UserName).NotEmpty().WithMessage("使用者名稱不可空白");

                RuleFor(x => x.Password)
               .NotEmpty()
               .WithMessage("密碼不可空白")
               .Must(value => RegExpUtil.IsMatch(RegExpUtil.RegexSymbolsEnglishNumbers, value))
               .WithMessage("請輸入由英文、數字或符號組成的密碼");
            }
        }
    }
}
