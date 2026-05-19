using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class RedBelt() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const int MaxConsume = 5;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK")
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ConsumeVar(MaxConsume),
        new ProduceVar(MaxConsume)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int consumed = 0;

        for (int i = 0; i < MaxConsume; i++)
        {
            bool didConsume = await MaterialHelper.ConsumeMaterial(
                Owner,
                choiceContext,
                1,
                MaterialSource.Stock,
                this);

            if (!didConsume)
                break;

            consumed++;
        }

        if (consumed > 0)
        {
            await MaterialHelper.ProduceMaterial(
                Owner,
                choiceContext,
                consumed,
                MaterialDestination.Hand,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}