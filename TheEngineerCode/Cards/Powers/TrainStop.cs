using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TheEngineerCardPool))]
public class TrainStop() : TheEngineerCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Wagon)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<TrainStopPower>(1),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.Apply<TrainStopPower>(Owner.Creature, this, DynamicVars.Power<TrainStopPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}