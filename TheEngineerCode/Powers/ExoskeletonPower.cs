using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Cards.Skills;

namespace TheEngineer.TheEngineerCode.Powers;

public class ExoskeletonPower:TemporaryDexterityPower, ICustomPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<Exoskeleton>();
    protected override bool IsPositive => true;
}