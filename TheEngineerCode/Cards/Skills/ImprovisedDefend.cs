using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class ImprovisedDefend() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self), IAncientAwakenedCard
{
    public override CardPoolModel VisualCardPool => AncientsAwakenedInterop.PerfectedCardPool(Pool);
    
    private const decimal BASE_BLOCK = 8;
    private const decimal UPGRADE_BLOCK = 3;
    
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Defend,
        TheEngineerCardTags.Wagon
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        TheEngineerKeyWords.Wagon
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BASE_BLOCK, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DynamicCombatHoverTip(owner =>
            WagonSingleton.Instance
                .GetLastWagon(owner))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}