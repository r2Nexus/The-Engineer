using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;


[Pool(typeof(TheEngineerCardPool))]
public class PurpleScience() : TheEngineerCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_POWER = 1;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-PRODUCEALL")
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Science];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PurpleSciencePower>(BASE_POWER)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<PurpleSciencePower>(this,DynamicVars.Power<PurpleSciencePower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}