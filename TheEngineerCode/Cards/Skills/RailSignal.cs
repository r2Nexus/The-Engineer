using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
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
    private const decimal BASE_PRODUCE = 1m;
    private const decimal UPGRADE_PRODUCE = 0m;

    private const decimal BASE_BLOCK = 6m;
    private const decimal UPGRADE_BLOCK = 3m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Wagon)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<RailSignalPower>(BASE_BLOCK),
        new ProduceVar(BASE_PRODUCE)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand,
            this);

        await CommonActions.ApplySelf<RailSignalPower>(
            this,
            DynamicVars.Power<RailSignalPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        //DynamicVars.Produce().UpgradeValueBy(UPGRADE_PRODUCE);
        DynamicVars.Power<RailSignalPower>().UpgradeValueBy(UPGRADE_BLOCK);
    }
}