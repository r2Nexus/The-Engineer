using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Cards.Skills;

namespace TheEngineer.TheEngineerCode.Powers;

public class PersonalBatteryPower : TemporaryFocusPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<PersonalBattery>();
}