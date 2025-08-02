
using FlapKap.Application.DTOs.Transaction;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlapKap.Application.Validators.Transaction;
public class DepositCoinsDtoValidator : AbstractValidator<DepositCoinsDto>
{
    public DepositCoinsDtoValidator()
    {
        RuleFor(x => x.FiveCent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TenCent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TwentyCent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FiftyCent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.HundredCent).GreaterThanOrEqualTo(0);

        RuleFor(x => x.Total)
            .GreaterThan(0)
            .WithMessage("Deposit must be greater than 0.");
    }
}