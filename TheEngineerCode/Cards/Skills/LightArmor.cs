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
public class LightArmor() : TheEngineerCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 7m;
    private const decimal UPGRADE_BLOCK = 2m;

    private const decimal CONSUEM = 1m;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new ConsumeVar(CONSUEM)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK")
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        var consumed = await MaterialHelper.ConsumeMaterial(this, choiceContext, 1, MaterialSource.Stock, play);
        if (consumed) await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}