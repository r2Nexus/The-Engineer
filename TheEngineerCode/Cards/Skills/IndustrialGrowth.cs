using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class IndustrialGrowth() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Ancient,
    TargetType.AnyEnemy), IOnProduced, IAncientAwakenedCard
{
    private const int BASE_OIL = 10;
    private const int BASE_SCALING = 2;
    private const int UPGRADE_SCALING = 1;

    private int _produceCount = 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..MakeCalculatedVar(
            "CalculatedOil",
            BASE_OIL,
            static (card, _) => ((IndustrialGrowth)card)._produceCount,
            BASE_SCALING
        )
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>(),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL")
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (play.Target != null)
            await CommonActions.Apply<OilPower>(choiceContext, play.Target, this,
                DynamicVars["CalculatedOil"]._previewValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CalculatedOilExtra"]
            .UpgradeValueBy(UPGRADE_SCALING);
    }

    public Task OnProduced(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialDestination destination,
        AbstractModel? causedBy)
    {
        if (player == Owner && amount > 0)
            _produceCount += amount;

        return Task.CompletedTask;
    }
}