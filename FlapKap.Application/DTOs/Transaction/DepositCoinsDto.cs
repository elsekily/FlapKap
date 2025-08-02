using FlapKap.Application.Enums;

namespace FlapKap.Application.DTOs.Transaction;

public class DepositCoinsDto
{
    public int FiveCent { get; set; }
    public int TenCent { get; set; }
    public int TwentyCent { get; set; }
    public int FiftyCent { get; set; }
    public int HundredCent { get; set; }

    public int Total =>
        (FiveCent * (int)CoinsEnum.Five) +
        (TenCent * (int)CoinsEnum.Ten) +
        (TwentyCent * (int)CoinsEnum.Twenty) +
        (FiftyCent * (int)CoinsEnum.Fifty) +
        (HundredCent * (int)CoinsEnum.Hundred);
}