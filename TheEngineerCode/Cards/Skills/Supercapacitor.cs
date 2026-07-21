using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class Supercapacitor() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CHARGE_MAX"),
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Charge)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6,ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        await ChargeHelper.OnConsumed(choiceContext, Owner, 2, MaterialSource.Hand, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}