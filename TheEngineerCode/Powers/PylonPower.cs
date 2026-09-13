using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Cards.Skills;
using TheEngineer.TheEngineerCode.Extensions;

namespace TheEngineer.TheEngineerCode.Powers;

public class PylonPower : TemporaryFocusPower, ICustomPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<PersonalBattery>();
    protected override bool IsPositive => true;
    public string? CustomPackedIconPath => "pylon_power.png".PowerImagePath();
    public string? CustomBigIconPath => "pylon_power.png".BigPowerImagePath();
}