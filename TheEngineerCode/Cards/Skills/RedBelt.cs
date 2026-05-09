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
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL"),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK"),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-PRODUCEALL")
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int consumed = await MaterialHelper.ConsumeAllMaterial(
            Owner,
            choiceContext,
            MaterialSource.Stock,
            this);
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