using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class RailSignal() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal REDUCTION = 1m;

    private const decimal BASE_BLOCK = 6m;
    private const decimal UPGRADE_BLOCK = 3m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Wagon)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<RailSignalPower>(REDUCTION),
        new BlockVar(BASE_BLOCK, ValueProp.Move)
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

        await CommonActions.ApplySelf<RailSignalPower>(
            choiceContext, 
            this, 
            DynamicVars.Power<RailSignalPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}