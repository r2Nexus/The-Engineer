using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Relics;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Relics;

[Pool(typeof(TheEngineerRelicPool))]
public class Arcosphere() : TheEngineerRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(1)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1)
    ];
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if(side != CombatSide.Player) return;
        int hand = MaterialHelper.CountMaterial(Owner, MaterialSource.Hand);
        int draw = MaterialHelper.CountMaterial(Owner, MaterialSource.Draw);
        int discard = MaterialHelper.CountMaterial(Owner, MaterialSource.Discard);

        if (hand >= 2 || draw >= 2 || discard >= 2)
        {
            Flash();
            await CommonActions.Apply<StrengthPower>(choiceContext, Owner.Creature,this);
        }
    }
}