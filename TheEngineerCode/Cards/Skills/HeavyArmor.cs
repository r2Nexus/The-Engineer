using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class HeavyArmor() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 8m;
    private const decimal UPGRADE_BLOCK = 0m;

    private const decimal BASE_PLATED = 4m;
    private const decimal UPGRADE_PLATED = 1m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlatingPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new PowerVar<PlatingPower>(BASE_PLATED),
        new ConsumeVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, DynamicVars.Block, play);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            2,
            MaterialSource.Hand,
            play);

        if (consumed)
        {
            await CommonActions.ApplySelf<PlatingPower>(
                this,
                DynamicVars.Power<PlatingPower>().BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        //DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
        DynamicVars.Power<PlatingPower>().UpgradeValueBy(UPGRADE_PLATED);
    }
}